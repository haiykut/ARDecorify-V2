package com.haiykut.ardecorifywebapi.service;

import com.haiykut.ardecorifywebapi.dto.request.unity.UnityAddOrderRequestBodyDto;
import com.haiykut.ardecorifywebapi.dto.request.unity.UnityAddOrderRequestDto;
import com.haiykut.ardecorifywebapi.dto.request.unity.UnityCustomerAuthenticationRequestDto;
import com.haiykut.ardecorifywebapi.dto.response.unity.mobile.UnityAddOrderResponseDto;
import com.haiykut.ardecorifywebapi.dto.response.unity.mobile.UnityCustomerAuthenticationResponseDto;
import com.haiykut.ardecorifywebapi.model.Customer;
import com.haiykut.ardecorifywebapi.model.Order;
import com.haiykut.ardecorifywebapi.model.OrderableFurniture;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;

@Service
@RequiredArgsConstructor
public class UnityService {
    private final CustomerService customerService;
    private final FurnitureService furnitureService;
    private final OrderService orderService;
    //Mobile
    public UnityAddOrderResponseDto addOrder(UnityAddOrderRequestDto unityOrderRequestDto){
        Customer orderedBy = customerService.getCustomerByIdForUnity(unityOrderRequestDto.getOrderedBy());
        Order newOrder = new Order();
        newOrder.setOrderedBy(orderedBy);
        newOrder.setFurnitures(getOrderableFurnitures(unityOrderRequestDto));
        orderService.save(newOrder);
        UnityAddOrderResponseDto response = new UnityAddOrderResponseDto();
        if(newOrder != null)
            response.setMessage("Ordered Succesfully!");
        else
            response.setMessage("Failed to order!");

        return response;
    }
    public List<OrderableFurniture> getOrderableFurnitures(UnityAddOrderRequestDto unityOrderRequestDto){
        List<OrderableFurniture> orderedFurnitures = new ArrayList<OrderableFurniture>();
        for(UnityAddOrderRequestBodyDto body : unityOrderRequestDto.getFurnitures()){
            OrderableFurniture orderableFurniture = new OrderableFurniture();
            orderableFurniture.setFurniture(furnitureService.getFurnitureForUnityById(body.getId()));
            orderableFurniture.setTransform(body.getPosition().getX(), body.getPosition().getY(), body.getPosition().getZ(), body.getRotation().getX(), body.getRotation().getY(), body.getRotation().getZ());
            orderedFurnitures.add(orderableFurniture);
        }
        return orderedFurnitures;
    }
    public UnityCustomerAuthenticationResponseDto authenticateUnity(UnityCustomerAuthenticationRequestDto userRequestDto){
        List<Customer> users = customerService.getCustomersForUnity();
        StringBuilder username = new StringBuilder(userRequestDto.getCustomer().getUsername());
        StringBuilder password = new StringBuilder(userRequestDto.getCustomer().getPassword());
        Boolean authenticate = false;
        Long userId = (long) -1;
        for(Customer customer : users){
            if(customer.getUsername().contentEquals(username.toString()) && customer.getPassword().contentEquals(password.toString())){
                authenticate = true;
                userId = customer.getCustomerId();
            }
        }
        String response = authenticate.toString();
        return new UnityCustomerAuthenticationResponseDto(response, userId);
    }
}
