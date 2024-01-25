package com.haiykut.ardecorifywebapi.controller;
import com.haiykut.ardecorifywebapi.dto.request.FurnitureRequestDto;
import com.haiykut.ardecorifywebapi.dto.response.FurnitureResponseDto;
import com.haiykut.ardecorifywebapi.service.FurnitureService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
@RestController
@RequestMapping("/api/furniture")
@RequiredArgsConstructor
public class FurnitureController {
    private final FurnitureService furnitureService;
    @GetMapping
    public ResponseEntity<List<FurnitureResponseDto>> getFurnitures(){
        return ResponseEntity.ok(furnitureService.getFurnitures());
    }
    @GetMapping("/{id}")
    public ResponseEntity<FurnitureResponseDto> getFurnitureById(@PathVariable Long id){
        return ResponseEntity.ok(furnitureService.getFurnitureById(id));
    }
    @PostMapping("/add")
    public ResponseEntity<FurnitureResponseDto> addFurniture(@RequestBody FurnitureRequestDto furnitureRequestDto){
        return ResponseEntity.ok(furnitureService.addFurniture(furnitureRequestDto));
    }
    @PutMapping("/update/{id}")
    public ResponseEntity<FurnitureResponseDto> updateFurnitureById(@PathVariable Long id, @RequestBody FurnitureRequestDto furnitureRequestDto){
        return ResponseEntity.ok(furnitureService.updateFurnitureById(id, furnitureRequestDto));
    }
    @DeleteMapping("/delete/{id}")
    public ResponseEntity<String> deleteFurnitureById(@PathVariable Long id){
        furnitureService.deleteFurnitureById(id);
        return new ResponseEntity<>("Furniture Deleted!", HttpStatus.OK);
    }
    @DeleteMapping("/delete/all")
    public ResponseEntity<String> deleteFurnitures(){
        furnitureService.deleteFurnitures();
        return new ResponseEntity<>("Furnitures Deleted!", HttpStatus.OK);
    }
}
