package com.haiykut.ardecorifywebapi.services.concretes;
import com.haiykut.ardecorifywebapi.configurations.MapperConfig;
import com.haiykut.ardecorifywebapi.services.abstracts.FurnitureService;
import com.haiykut.ardecorifywebapi.services.abstracts.OrderService;
import com.haiykut.ardecorifywebapi.services.dtos.request.furniture.FurnitureAddRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.furniture.FurnitureGetRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.furniture.FurnitureUpdateRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.furniture.FurnitureAddResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.furniture.FurnitureGetResponseDto;
import com.haiykut.ardecorifywebapi.entities.Category;
import com.haiykut.ardecorifywebapi.entities.Furniture;
import com.haiykut.ardecorifywebapi.entities.Order;
import com.haiykut.ardecorifywebapi.repositories.FurnitureRepository;
import com.haiykut.ardecorifywebapi.services.dtos.response.furniture.FurnitureUpdateResponseDto;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.List;
import java.util.stream.Collectors;
@Service
@RequiredArgsConstructor
public class FurnitureServiceImpl implements FurnitureService {
    private final FurnitureRepository furnitureRepository;
    private final OrderService orderService;
    private final MapperConfig mapperConfig;
    @Override
    public FurnitureAddResponseDto addFurniture(FurnitureAddRequestDto furnitureRequestDto){
        Furniture requestedFurniture = new Furniture();
        requestedFurniture.setName(furnitureRequestDto.getName());
        Category category = new Category();
        category.setId(furnitureRequestDto.getCategoryId());
        requestedFurniture.setCategory(category);
        furnitureRepository.save(requestedFurniture);
        return mapperConfig.modelMapper().map(requestedFurniture, FurnitureAddResponseDto.class);
    }
    @Override
    public List<FurnitureGetResponseDto> getFurnitures(){
        List<Furniture> requestedFurnitures = furnitureRepository.findAll();
        List<FurnitureGetResponseDto> furnituresDto;
        furnituresDto = requestedFurnitures.stream()
                .map(furniture -> mapperConfig.modelMapper().map(furniture, FurnitureGetResponseDto.class))
                .collect(Collectors.toList());
        return furnituresDto;
    }
    @Override
    public FurnitureGetResponseDto getFurnitureById(Long id){
        Furniture requestedFurniture = furnitureRepository.findById(id).orElseThrow();
        FurnitureGetResponseDto furnitureResponseDto;
        furnitureResponseDto = mapperConfig.modelMapper().map(requestedFurniture, FurnitureGetResponseDto.class);
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
    public FurnitureUpdateResponseDto updateFurnitureById(Long id, FurnitureUpdateRequestDto furnitureRequestDto){
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
        return mapperConfig.modelMapper().map(requestedFurniture, FurnitureUpdateResponseDto.class);
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