package com.haiykut.ardecorifywebapi.controllers;
import com.haiykut.ardecorifywebapi.services.abstracts.OrderService;
import com.haiykut.ardecorifywebapi.services.dtos.response.order.OrderResponseDto;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.config.annotation.EnableWebMvc;
import java.util.List;
@RestController
@RequestMapping("/api/order")
@RequiredArgsConstructor
@EnableWebMvc
public class OrderController {
    private final OrderService orderService;
    @GetMapping
    public ResponseEntity<List<OrderResponseDto>> getOrders(){
        return ResponseEntity.ok(orderService.getOrders());
    }
    @GetMapping("/{id}")
    public ResponseEntity<OrderResponseDto> getOrderById(@PathVariable Long id){
        return ResponseEntity.ok(orderService.getOrderById(id));
    }
    @DeleteMapping("/delete/{id}")
    public ResponseEntity<String> deleteOrderById(@PathVariable Long id){
        orderService.deleteOrderById(id);
        return new ResponseEntity<>("Order Deleted!", HttpStatus.OK);
    }
    @DeleteMapping("/delete/all")
    public ResponseEntity<String> deleteOrders(){
        orderService.deleteOrders();
        return new ResponseEntity<>("Orders Deleted!", HttpStatus.OK);
    }
}
