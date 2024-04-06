package com.haiykut.ardecorifywebapi.services.concretes;
import com.haiykut.ardecorifywebapi.configurations.MapperConfig;
import com.haiykut.ardecorifywebapi.services.abstracts.CategoryService;
import com.haiykut.ardecorifywebapi.services.abstracts.OrderService;
import com.haiykut.ardecorifywebapi.services.dtos.request.category.CategoryAddRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.category.CategoryUpdateRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.category.CategoryAddResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.category.CategoryGetResponseDto;
import com.haiykut.ardecorifywebapi.entities.Category;
import com.haiykut.ardecorifywebapi.entities.Order;
import com.haiykut.ardecorifywebapi.repositories.CategoryRepository;
import com.haiykut.ardecorifywebapi.services.dtos.response.category.CategoryUpdateResponseDto;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.List;
import java.util.stream.Collectors;
@Service
@RequiredArgsConstructor
public class CategoryServiceImpl implements CategoryService {
    private final CategoryRepository categoryRepository;
    private final OrderService orderService;
    private final MapperConfig mapperConfig;
    @Override
    public CategoryGetResponseDto getCategoryById(Long id){
        Category requestedCategory = categoryRepository.findById(id).orElse(null);
        return mapperConfig.modelMapper().map(requestedCategory, CategoryGetResponseDto.class);
    }
    @Override
    public List<CategoryGetResponseDto> getCategories(){
        List<Category> requestedCategories = categoryRepository.findAll();
        List<CategoryGetResponseDto> categoryResponseDtos;
        categoryResponseDtos = requestedCategories.stream()
                .map(category -> mapperConfig.modelMapper().map(category, CategoryGetResponseDto.class))
                .collect(Collectors.toList());
        return categoryResponseDtos;
    }
    @Override
    public CategoryAddResponseDto addCategory(CategoryAddRequestDto categoryRequestDto){
        Category requestedCategory = new Category();
        requestedCategory.setName(categoryRequestDto.getName());
        categoryRepository.save(requestedCategory);
        return mapperConfig.modelMapper().map(requestedCategory, CategoryAddResponseDto.class);
    }
    @Override
    public CategoryUpdateResponseDto updateCategoryById(Long id, CategoryUpdateRequestDto categoryRequestDto){
        Category requestedCategory = categoryRepository.findById(id).orElseThrow();
        requestedCategory.setName(categoryRequestDto.getName());
        categoryRepository.save(requestedCategory);
        return mapperConfig.modelMapper().map(requestedCategory, CategoryUpdateResponseDto.class);
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
