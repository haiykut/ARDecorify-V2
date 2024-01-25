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
    public ResponseEntity<List<FurnitureResponseDto>> getAllFurnitures(){
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
    public ResponseEntity<FurnitureResponseDto> updateFurniture(@PathVariable Long id, @RequestBody FurnitureRequestDto furnitureRequestDto){
        return ResponseEntity.ok(furnitureService.updateFurniture(id, furnitureRequestDto));
    }
    @DeleteMapping("/delete/{id}")
    public ResponseEntity<String> deleteFurniture(@PathVariable Long id){
        furnitureService.deleteFurniture(id);
        return new ResponseEntity<>("Furniture Deleted!", HttpStatus.OK);
    }
    @DeleteMapping("/delete/all")
    public ResponseEntity<String>deleteAllFurnitures(){
        furnitureService.deleteAllFurnitures();
        return new ResponseEntity<>("Furnitures Deleted!", HttpStatus.OK);
    }
}
