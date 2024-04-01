package com.haiykut.ardecorifywebapi.service;
import com.haiykut.ardecorifywebapi.configuration.MapperConfig;
import com.haiykut.ardecorifywebapi.dto.request.FurnitureRequestDto;
import com.haiykut.ardecorifywebapi.dto.response.FurnitureResponseDto;
import com.haiykut.ardecorifywebapi.model.Category;
import com.haiykut.ardecorifywebapi.model.Furniture;
import com.haiykut.ardecorifywebapi.model.Order;
import com.haiykut.ardecorifywebapi.repository.FurnitureRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.List;
import java.util.stream.Collectors;
@Service
@RequiredArgsConstructor
public class FurnitureService {
    private final FurnitureRepository furnitureRepository;
    private final OrderService orderService;
    private final MapperConfig mapperConfig;
    public FurnitureResponseDto addFurniture(FurnitureRequestDto furnitureRequestDto){
        Furniture requestedFurniture = new Furniture();
        requestedFurniture.setName(furnitureRequestDto.getName());
        Category category = new Category();
        category.setId(furnitureRequestDto.getCategoryId());
        requestedFurniture.setCategory(category);
        furnitureRepository.save(requestedFurniture);
        return mapperConfig.modelMapper().map(requestedFurniture, FurnitureResponseDto.class);
    }
    public List<FurnitureResponseDto> getFurnitures(){
        List<Furniture> requestedFurnitures = furnitureRepository.findAll();
        List<FurnitureResponseDto> furnituresDto;
        furnituresDto = requestedFurnitures.stream()
                .map(furniture -> mapperConfig.modelMapper().map(furniture, FurnitureResponseDto.class))
                .collect(Collectors.toList());
        return furnituresDto;
    }
    public FurnitureResponseDto getFurnitureById(Long id){
        Furniture requestedFurniture = furnitureRepository.findById(id).orElseThrow();
        FurnitureResponseDto furnitureResponseDto;
        furnitureResponseDto = mapperConfig.modelMapper().map(requestedFurniture, FurnitureResponseDto.class);
        return furnitureResponseDto;
    }
    public void deleteFurnitureById(Long id){
        Furniture requestedFurniture = furnitureRepository.findById(id).orElseThrow();
        checkOrder(id);
        furnitureRepository.delete(requestedFurniture);
    }
    public void deleteFurnitures(){
        checkOrders();
        furnitureRepository.deleteAll();
    }
    public FurnitureResponseDto updateFurnitureById(Long id, FurnitureRequestDto furnitureRequestDto){
        Furniture requestedFurniture = furnitureRepository.findById(id)
                .orElseThrow();
        requestedFurniture.setName(furnitureRequestDto.getName());
        if (furnitureRequestDto.getCategoryId() != null) {
            Category updatedCategory = new Category();
            updatedCategory.setId(furnitureRequestDto.getCategoryId());
            requestedFurniture.setCategory(updatedCategory);
            furnitureRepository.save(requestedFurniture);
        } else {
            requestedFurniture.setCategory(null);
        }
        return mapperConfig.modelMapper().map(requestedFurniture, FurnitureResponseDto.class);
    }
    public Furniture getFurnitureForUnityById(Long id){
        return furnitureRepository.findById(id).orElseThrow();
    }
    public void checkOrders(){
        if(orderService.getOrders() != null){
            orderService.deleteOrders();
        }
    }
    public void checkOrder(Long id){
        List<Order> orders = orderService.getOrdersForFurnitureService();
        if(orders != null){
            for(Order order : orders){
                order.getFurnitures().removeIf(orderableFurniture -> orderableFurniture.getFurniture().getId().longValue() == id.longValue());
            }
        }
    }
}