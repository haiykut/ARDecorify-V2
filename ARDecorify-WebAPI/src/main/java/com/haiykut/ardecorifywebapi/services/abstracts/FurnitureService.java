package com.haiykut.ardecorifywebapi.services.abstracts;
import com.haiykut.ardecorifywebapi.entities.Furniture;
import com.haiykut.ardecorifywebapi.services.dtos.request.furniture.FurnitureAddRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.furniture.FurnitureGetRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.furniture.FurnitureUpdateRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.furniture.FurnitureAddResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.furniture.FurnitureGetResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.furniture.FurnitureUpdateResponseDto;

import java.util.List;
public interface FurnitureService {
    FurnitureAddResponseDto addFurniture(FurnitureAddRequestDto furnitureRequestDto);
    List<FurnitureGetResponseDto> getFurnitures();
    FurnitureGetResponseDto getFurnitureById(Long id);
    void deleteFurnitureById(Long id);
    void deleteFurnitures();
    FurnitureUpdateResponseDto updateFurnitureById(Long id, FurnitureUpdateRequestDto furnitureRequestDto);
    Furniture getFurnitureForUnityById(Long id);
    void checkOrders();
    void checkOrder(Long id);
}
