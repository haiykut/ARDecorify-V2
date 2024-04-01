package com.haiykut.ardecorifywebapi.services.abstracts;
import com.haiykut.ardecorifywebapi.entities.Order;
import com.haiykut.ardecorifywebapi.services.dtos.response.OrderResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.OrderableFurnitureResponseDto;
import java.util.List;
public interface OrderService {
    List<OrderResponseDto> getOrders();
    List<OrderableFurnitureResponseDto> getOrderableFurnitures(Order order);
    void save(Order newOrder);
    OrderResponseDto getOrderById(Long id);
    void deleteOrderById(Long id);
    void deleteOrders();
    Order getOrderForUnityWebGL(Long id);
    List<Order> getOrdersForFurnitureService();
}
