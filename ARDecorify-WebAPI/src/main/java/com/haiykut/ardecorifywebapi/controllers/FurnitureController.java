package com.haiykut.ardecorifywebapi.controllers;
import com.haiykut.ardecorifywebapi.services.abstracts.FurnitureService;
import com.haiykut.ardecorifywebapi.services.dtos.request.furniture.FurnitureAddRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.furniture.FurnitureGetRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.furniture.FurnitureUpdateRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.furniture.FurnitureAddResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.furniture.FurnitureGetResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.furniture.FurnitureUpdateResponseDto;
import jakarta.validation.Valid;
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
    public ResponseEntity<List<FurnitureGetResponseDto>> getFurnitures(){
        return ResponseEntity.ok(furnitureService.getFurnitures());
    }
    @GetMapping("/{id}")
    public ResponseEntity<FurnitureGetResponseDto> getFurnitureById(@PathVariable Long id){
        return ResponseEntity.ok(furnitureService.getFurnitureById(id));
    }
    @PostMapping("/add")
    public ResponseEntity<FurnitureAddResponseDto> addFurniture(@Valid @RequestBody FurnitureAddRequestDto furnitureRequestDto){
        return ResponseEntity.ok(furnitureService.addFurniture(furnitureRequestDto));
    }
    @PutMapping("/update/{id}")
    public ResponseEntity<FurnitureUpdateResponseDto> updateFurnitureById(@Valid @PathVariable Long id, @RequestBody FurnitureUpdateRequestDto furnitureRequestDto){
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
