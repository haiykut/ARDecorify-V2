package com.haiykut.ardecorifywebapi.services.abstracts;
import com.haiykut.ardecorifywebapi.services.dtos.request.CategoryRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.CategoryResponseDto;
import java.util.List;
public interface CategoryService {
    CategoryResponseDto getCategoryById(Long id);
    List<CategoryResponseDto> getCategories();
    CategoryResponseDto addCategory(CategoryRequestDto categoryRequestDto);
    CategoryResponseDto updateCategoryById(Long id, CategoryRequestDto categoryRequestDto);
    void deleteCategoryById(Long id);
    void deleteCategories();
    void checkOrders();
    void checkOrder(Long id);
}
