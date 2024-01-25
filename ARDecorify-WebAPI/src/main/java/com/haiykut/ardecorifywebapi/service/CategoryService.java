package com.haiykut.ardecorifywebapi.service;
import com.haiykut.ardecorifywebapi.configuration.MapperConfig;
import com.haiykut.ardecorifywebapi.dto.request.CategoryRequestDto;
import com.haiykut.ardecorifywebapi.dto.response.CategoryResponseDto;
import com.haiykut.ardecorifywebapi.model.Category;
import com.haiykut.ardecorifywebapi.repository.CategoryRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.List;
import java.util.stream.Collectors;
@Service
@RequiredArgsConstructor
public class CategoryService {
    private final CategoryRepository categoryRepository;
    private final MapperConfig mapperConfig;
    public CategoryResponseDto getCategoryById(Long id){
        Category requestedCategory = categoryRepository.findById(id).orElse(null);
        return mapperConfig.modelMapper().map(requestedCategory, CategoryResponseDto.class);
    }
    public List<CategoryResponseDto> getCategories(){
        List<Category> requestedCategories = categoryRepository.findAll();
        List<CategoryResponseDto> categoryResponseDtos;
        categoryResponseDtos = requestedCategories.stream()
                .map(category -> mapperConfig.modelMapper().map(category, CategoryResponseDto.class))
                .collect(Collectors.toList());
        return categoryResponseDtos;
    }
    public CategoryResponseDto addCategory(CategoryRequestDto categoryRequestDto){
        Category requestedCategory = new Category();
        requestedCategory.setName(categoryRequestDto.getName());
        categoryRepository.save(requestedCategory);
        return mapperConfig.modelMapper().map(requestedCategory, CategoryResponseDto.class);
    }
    public CategoryResponseDto updateCategoryById(Long id, CategoryRequestDto categoryRequestDto){
        Category requestedCategory = categoryRepository.findById(id).orElseThrow();
        requestedCategory.setName(categoryRequestDto.getName());
        categoryRepository.save(requestedCategory);
        return mapperConfig.modelMapper().map(requestedCategory, CategoryResponseDto.class);
    }
    public void deleteCategoryById(Long id){
        Category requestedCategory = categoryRepository.findById(id).orElseThrow();
        categoryRepository.delete(requestedCategory);
    }
    public void deleteCategories(){
        categoryRepository.deleteAll();
    }
}
