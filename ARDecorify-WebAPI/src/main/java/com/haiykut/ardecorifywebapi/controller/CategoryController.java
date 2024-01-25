package com.haiykut.ardecorifywebapi.controller;
import com.haiykut.ardecorifywebapi.dto.request.CategoryRequestDto;
import com.haiykut.ardecorifywebapi.dto.response.CategoryResponseDto;
import com.haiykut.ardecorifywebapi.service.CategoryService;
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
    public ResponseEntity<List<CategoryResponseDto>> getAllCategories(){
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
    public ResponseEntity<CategoryResponseDto> updateCategory(@PathVariable Long id, @RequestBody CategoryRequestDto categoryRequestDto){
        return ResponseEntity.ok(categoryService.updateCategory(id, categoryRequestDto));
    }
    @DeleteMapping("/delete/{id}")
    public ResponseEntity<String> deleteCategoryById(@PathVariable Long id){
        categoryService.deleteCategory(id);
        return new ResponseEntity<>("Category Deleted!", HttpStatus.OK);
    }
    @DeleteMapping("/delete/all")
    public ResponseEntity<String> deleteAll(){
        categoryService.deleteAllCategories();
        return new ResponseEntity<>("All Categories Deleted!", HttpStatus.OK);
    }
}
