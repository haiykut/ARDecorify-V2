package com.haiykut.ardecorifywebapi.services.concretes;
import com.haiykut.ardecorifywebapi.configuration.MapperConfig;
import com.haiykut.ardecorifywebapi.services.abstracts.CategoryService;
import com.haiykut.ardecorifywebapi.services.dtos.request.CategoryRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.CategoryResponseDto;
import com.haiykut.ardecorifywebapi.entities.Category;
import com.haiykut.ardecorifywebapi.entities.Order;
import com.haiykut.ardecorifywebapi.repositories.CategoryRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.List;
import java.util.stream.Collectors;
@Service
@RequiredArgsConstructor
public class CategoryServiceImpl implements CategoryService {
    private final CategoryRepository categoryRepository;
    private final OrderServiceImpl orderService;
    private final MapperConfig mapperConfig;
    @Override
    public CategoryResponseDto getCategoryById(Long id){
        Category requestedCategory = categoryRepository.findById(id).orElse(null);
        return mapperConfig.modelMapper().map(requestedCategory, CategoryResponseDto.class);
    }
    @Override
    public List<CategoryResponseDto> getCategories(){
        List<Category> requestedCategories = categoryRepository.findAll();
        List<CategoryResponseDto> categoryResponseDtos;
        categoryResponseDtos = requestedCategories.stream()
                .map(category -> mapperConfig.modelMapper().map(category, CategoryResponseDto.class))
                .collect(Collectors.toList());
        return categoryResponseDtos;
    }
    @Override
    public CategoryResponseDto addCategory(CategoryRequestDto categoryRequestDto){
        Category requestedCategory = new Category();
        requestedCategory.setName(categoryRequestDto.getName());
        categoryRepository.save(requestedCategory);
        return mapperConfig.modelMapper().map(requestedCategory, CategoryResponseDto.class);
    }
    @Override
    public CategoryResponseDto updateCategoryById(Long id, CategoryRequestDto categoryRequestDto){
        Category requestedCategory = categoryRepository.findById(id).orElseThrow();
        requestedCategory.setName(categoryRequestDto.getName());
        categoryRepository.save(requestedCategory);
        return mapperConfig.modelMapper().map(requestedCategory, CategoryResponseDto.class);
    }
    @Override
    public void deleteCategoryById(Long id){
        Category requestedCategory = categoryRepository.findById(id).orElseThrow();
        checkOrder(id);
        categoryRepository.delete(requestedCategory);
    }
    @Override
    public void deleteCategories(){
        checkOrders();
        categoryRepository.deleteAll();
    }
    @Override
    public void checkOrders(){
        if(orderService.getOrders() != null){
            orderService.deleteOrders();
        }
    }
    @Override
    public void checkOrder(Long id){
        List<Order> orders = orderService.getOrdersForFurnitureService();
        for(Order order : orders){
            order.getFurnitures().removeIf(orderableFurniture -> orderableFurniture.getFurniture().getId().longValue() == id.longValue());
        }
    }
}
