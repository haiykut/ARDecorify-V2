package com.haiykut.ardecorifywebapi.controllers;
import com.haiykut.ardecorifywebapi.services.abstracts.CategoryService;
import com.haiykut.ardecorifywebapi.services.dtos.request.CategoryRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.CategoryResponseDto;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import java.util.List;
@RestController
@RequestMapping("/api/category")
@RequiredArgsConstructor
public class CategoryController {
    private final CategoryService categoryService;
    @GetMapping
    public ResponseEntity<List<CategoryResponseDto>> getCategories(){
        return new ResponseEntity<>(categoryService.getCategories(), HttpStatus.OK);
    }
    @GetMapping("/{id}")
    public ResponseEntity<CategoryResponseDto> getCategoryById(@PathVariable Long id){
        return ResponseEntity.ok(categoryService.getCategoryById(id));
    }
    @PostMapping("/add")
    public  ResponseEntity<CategoryResponseDto> addCategory(@RequestBody CategoryRequestDto categoryRequestDto){
        return ResponseEntity.ok(categoryService.addCategory(categoryRequestDto));
    }
    @PutMapping("/update/{id}")
    public ResponseEntity<CategoryResponseDto> updateCategoryById(@PathVariable Long id, @RequestBody CategoryRequestDto categoryRequestDto){
        return ResponseEntity.ok(categoryService.updateCategoryById(id, categoryRequestDto));
    }
    @DeleteMapping("/delete/{id}")
    public ResponseEntity<String> deleteCategoryById(@PathVariable Long id){
        categoryService.deleteCategoryById(id);
        return new ResponseEntity<>("Category Deleted!", HttpStatus.OK);
    }
    @DeleteMapping("/delete/all")
    public ResponseEntity<String> deleteCategories(){
        categoryService.deleteCategories();
        return new ResponseEntity<>("All Categories Deleted!", HttpStatus.OK);
    }
}
