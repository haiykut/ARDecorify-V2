package com.haiykut.ardecorifywebapi.service;
import com.haiykut.ardecorifywebapi.dto.response.OrderResponseDto;
import com.haiykut.ardecorifywebapi.dto.response.OrderableFurnitureResponseDto;
import com.haiykut.ardecorifywebapi.model.Order;
import com.haiykut.ardecorifywebapi.model.OrderableFurniture;
import com.haiykut.ardecorifywebapi.repository.OrderRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.ArrayList;
import java.util.List;
@Service
@RequiredArgsConstructor
public class OrderService {
    private final OrderRepository orderRepository;
    public List<OrderResponseDto> getOrders(){
        List<Order> allOrders = orderRepository.findAll();
        List<OrderResponseDto> allOrderDtos = new ArrayList<>();
        for(Order order : allOrders){
            OrderableFurnitureResponseDto orderableFurnitureResponseDto = new OrderableFurnitureResponseDto();
            orderableFurnitureResponseDto.setId(order.getOrderId());
            OrderResponseDto o = new OrderResponseDto(order.getOrderId(),order.getOrderedBy().getCustomerId(), getOrderableFurnitures(order));
            allOrderDtos.add(o);
        }
        return  allOrderDtos;
    }
    public List<OrderableFurnitureResponseDto> getOrderableFurnitures(Order order){
        List<OrderableFurnitureResponseDto> orderableFurnitureResponseDtos = new ArrayList<>();
        for(OrderableFurniture orderableFurniture : order.getFurnitures()){
            OrderableFurnitureResponseDto orderableFurnitureResponseDto = new OrderableFurnitureResponseDto(orderableFurniture.getOrderableFurnitureId(), orderableFurniture.getFurniture().getCategory().getName(), orderableFurniture.getPosX(), orderableFurniture.getPosY(), orderableFurniture.getPosZ(), orderableFurniture.getRotX(), orderableFurniture.getRotY(), orderableFurniture.getRotZ());
            orderableFurnitureResponseDtos.add(orderableFurnitureResponseDto);
        }
        return orderableFurnitureResponseDtos;
    }
    public void save(Order newOrder){
        orderRepository.save(newOrder);
    }
    public OrderResponseDto getOrderById(Long id){
        Order requestedOrder = orderRepository.findById(id).orElseThrow();
        return new OrderResponseDto(requestedOrder.getOrderId(),requestedOrder.getOrderedBy().getCustomerId(), getOrderableFurnitures(requestedOrder));
    }
    public void deleteOrderById(Long id){
        Order requestedOrder = orderRepository.findById(id).orElseThrow();
        orderRepository.delete(requestedOrder);
    }
    public void deleteOrders(){
        orderRepository.deleteAll();
    }
    public Order getOrderForUnityWebGL(Long id){
        return orderRepository.findById(id).orElseThrow();
    }
    public List<Order> getOrdersForFurnitureService(){
        return orderRepository.findAll();
    }
}
