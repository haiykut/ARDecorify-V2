package com.haiykut.ardecorifywebapi.services.abstracts;
import com.haiykut.ardecorifywebapi.entities.Furniture;
import com.haiykut.ardecorifywebapi.services.dtos.request.FurnitureRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.FurnitureResponseDto;
import java.util.List;
public interface FurnitureService {
    FurnitureResponseDto addFurniture(FurnitureRequestDto furnitureRequestDto);
    List<FurnitureResponseDto> getFurnitures();
    FurnitureResponseDto getFurnitureById(Long id);
    void deleteFurnitureById(Long id);
    void deleteFurnitures();
    FurnitureResponseDto updateFurnitureById(Long id, FurnitureRequestDto furnitureRequestDto);
    Furniture getFurnitureForUnityById(Long id);
    void checkOrders();
    void checkOrder(Long id);
}
