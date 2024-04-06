package com.haiykut.ardecorifywebapi.services.abstracts;
import com.haiykut.ardecorifywebapi.services.dtos.request.category.CategoryAddRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.category.CategoryUpdateRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.category.CategoryAddResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.category.CategoryGetResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.category.CategoryUpdateResponseDto;

import java.util.List;
public interface CategoryService {
    CategoryGetResponseDto getCategoryById(Long id);
    List<CategoryGetResponseDto> getCategories();
    CategoryAddResponseDto addCategory(CategoryAddRequestDto categoryRequestDto);
    CategoryUpdateResponseDto updateCategoryById(Long id, CategoryUpdateRequestDto categoryRequestDto);
    void deleteCategoryById(Long id);
    void deleteCategories();
    void checkOrders();
    void checkOrder(Long id);
}
