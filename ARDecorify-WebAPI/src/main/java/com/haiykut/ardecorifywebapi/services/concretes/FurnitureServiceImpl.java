package com.haiykut.ardecorifywebapi.services.concretes;
import com.haiykut.ardecorifywebapi.configurations.MapperConfig;
import com.haiykut.ardecorifywebapi.services.abstracts.FurnitureService;
import com.haiykut.ardecorifywebapi.services.dtos.request.FurnitureRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.FurnitureResponseDto;
import com.haiykut.ardecorifywebapi.entities.Category;
import com.haiykut.ardecorifywebapi.entities.Furniture;
import com.haiykut.ardecorifywebapi.entities.Order;
import com.haiykut.ardecorifywebapi.repositories.FurnitureRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.List;
import java.util.stream.Collectors;
@Service
@RequiredArgsConstructor
public class FurnitureServiceImpl implements FurnitureService {
    private final FurnitureRepository furnitureRepository;
    private final OrderServiceImpl orderService;
    private final MapperConfig mapperConfig;
    @Override
    public FurnitureResponseDto addFurniture(FurnitureRequestDto furnitureRequestDto){
        Furniture requestedFurniture = new Furniture();
        requestedFurniture.setName(furnitureRequestDto.getName());
        Category category = new Category();
        category.setId(furnitureRequestDto.getCategoryId());
        requestedFurniture.setCategory(category);
        furnitureRepository.save(requestedFurniture);
        return mapperConfig.modelMapper().map(requestedFurniture, FurnitureResponseDto.class);
    }
    @Override
    public List<FurnitureResponseDto> getFurnitures(){
        List<Furniture> requestedFurnitures = furnitureRepository.findAll();
        List<FurnitureResponseDto> furnituresDto;
        furnituresDto = requestedFurnitures.stream()
                .map(furniture -> mapperConfig.modelMapper().map(furniture, FurnitureResponseDto.class))
                .collect(Collectors.toList());
        return furnituresDto;
    }
    @Override
    public FurnitureResponseDto getFurnitureById(Long id){
        Furniture requestedFurniture = furnitureRepository.findById(id).orElseThrow();
        FurnitureResponseDto furnitureResponseDto;
        furnitureResponseDto = mapperConfig.modelMapper().map(requestedFurniture, FurnitureResponseDto.class);
        return furnitureResponseDto;
    }
    @Override
    public void deleteFurnitureById(Long id){
        Furniture requestedFurniture = furnitureRepository.findById(id).orElseThrow();
        checkOrder(id);
        furnitureRepository.delete(requestedFurniture);
    }
    @Override
    public void deleteFurnitures(){
        checkOrders();
        furnitureRepository.deleteAll();
    }
    @Override
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
    @Override
    public Furniture getFurnitureForUnityById(Long id){
        return furnitureRepository.findById(id).orElseThrow();
    }
    @Override
    public void checkOrders(){
        if(orderService.getOrders() != null){
            orderService.deleteOrders();
        }
    }
    @Override
    public void checkOrder(Long id){
        List<Order> orders = orderService.getOrdersForFurnitureService();
        if(orders != null){
            for(Order order : orders){
                order.getFurnitures().removeIf(orderableFurniture -> orderableFurniture.getFurniture().getId().longValue() == id.longValue());
            }
        }
    }
}