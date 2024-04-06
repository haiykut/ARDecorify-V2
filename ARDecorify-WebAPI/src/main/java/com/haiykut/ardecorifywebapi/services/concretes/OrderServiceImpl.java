package com.haiykut.ardecorifywebapi.services.concretes;
import com.haiykut.ardecorifywebapi.services.abstracts.OrderService;
import com.haiykut.ardecorifywebapi.services.dtos.response.order.OrderResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.orderablefurniture.OrderableFurnitureResponseDto;
import com.haiykut.ardecorifywebapi.entities.Order;
import com.haiykut.ardecorifywebapi.entities.OrderableFurniture;
import com.haiykut.ardecorifywebapi.repositories.OrderRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.ArrayList;
import java.util.List;
@Service
@RequiredArgsConstructor
public class OrderServiceImpl implements OrderService {

    private final OrderRepository orderRepository;
    @Override
    public List<OrderResponseDto> getOrders(){
        List<Order> allOrders = orderRepository.findAll();
        List<OrderResponseDto> allOrderDtos = new ArrayList<>();
        for(Order order : allOrders){
            OrderableFurnitureResponseDto orderableFurnitureResponseDto = new OrderableFurnitureResponseDto();
            orderableFurnitureResponseDto.setId(order.getId());
            OrderResponseDto o = new OrderResponseDto(order.getId(),order.getOrderedBy().getId(), getOrderableFurnitures(order));
            allOrderDtos.add(o);
        }
        return  allOrderDtos;
    }
    @Override
    public List<OrderableFurnitureResponseDto> getOrderableFurnitures(Order order){
        List<OrderableFurnitureResponseDto> orderableFurnitureResponseDtos = new ArrayList<>();
        for(OrderableFurniture orderableFurniture : order.getFurnitures()){
            OrderableFurnitureResponseDto orderableFurnitureResponseDto = new OrderableFurnitureResponseDto(orderableFurniture.getFurniture().getId(), orderableFurniture.getFurniture().getCategory().getName(), orderableFurniture.getPosX(), orderableFurniture.getPosY(), orderableFurniture.getPosZ(), orderableFurniture.getRotX(), orderableFurniture.getRotY(), orderableFurniture.getRotZ());
            orderableFurnitureResponseDtos.add(orderableFurnitureResponseDto);
        }
        return orderableFurnitureResponseDtos;
    }
    @Override
    public void save(Order newOrder){
        orderRepository.save(newOrder);
    }
    @Override
    public OrderResponseDto getOrderById(Long id){
        Order requestedOrder = orderRepository.findById(id).orElseThrow();
        return new OrderResponseDto(requestedOrder.getId(),requestedOrder.getOrderedBy().getId(), getOrderableFurnitures(requestedOrder));
    }
    @Override
    public void deleteOrderById(Long id){
        Order requestedOrder = orderRepository.findById(id).orElseThrow();
        orderRepository.delete(requestedOrder);
    }
    @Override
    public void deleteOrders(){
        orderRepository.deleteAll();
    }
    @Override
    public Order getOrderForUnityWebGL(Long id){
        return orderRepository.findById(id).orElseThrow();
    }
    @Override
    public List<Order> getOrdersForFurnitureService(){
        return orderRepository.findAll();
    }
}
