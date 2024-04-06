package com.haiykut.ardecorifywebapi.services.concretes;
import com.haiykut.ardecorifywebapi.services.abstracts.CustomerService;
import com.haiykut.ardecorifywebapi.services.abstracts.FurnitureService;
import com.haiykut.ardecorifywebapi.services.abstracts.OrderService;
import com.haiykut.ardecorifywebapi.services.abstracts.UnityService;
import com.haiykut.ardecorifywebapi.services.dtos.request.unity.UnityAddOrderRequestBodyDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.unity.UnityAddOrderRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.unity.UnityCustomerAuthenticationRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.unity.mobile.UnityAddOrderResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.unity.mobile.UnityCustomerAuthenticationResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.unity.webgl.UnityGetOrderResponseBodyDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.unity.webgl.UnityGetOrderResponseDto;
import com.haiykut.ardecorifywebapi.entities.Customer;
import com.haiykut.ardecorifywebapi.entities.Order;
import com.haiykut.ardecorifywebapi.entities.OrderableFurniture;
import com.haiykut.ardecorifywebapi.services.dtos.Vector3;
import lombok.RequiredArgsConstructor;
import org.springframework.core.io.ClassPathResource;
import org.springframework.core.io.Resource;
import org.springframework.stereotype.Service;
import java.io.IOException;
import java.nio.file.Files;
import java.util.ArrayList;
import java.util.List;
@Service
@RequiredArgsConstructor
public class UnityServiceImpl implements UnityService {
    private final CustomerService customerService;
    private final FurnitureService furnitureService;
    private final OrderService orderService;
    //Mobile
    @Override
    public UnityAddOrderResponseDto addOrder(UnityAddOrderRequestDto unityOrderRequestDto){
        Customer orderedBy = customerService.getCustomerByIdForUnity(unityOrderRequestDto.getOrderedBy());
        Order newOrder = new Order();
        newOrder.setOrderedBy(orderedBy);
        newOrder.setFurnitures(getOrderableFurnitures(unityOrderRequestDto));
        orderService.save(newOrder);
        UnityAddOrderResponseDto response = new UnityAddOrderResponseDto();
        response.setMessage("Ordered Succesfully!");
        return response;
    }
    @Override
    public List<OrderableFurniture> getOrderableFurnitures(UnityAddOrderRequestDto unityOrderRequestDto){
        List<OrderableFurniture> orderedFurnitures = new ArrayList<>();
        for(UnityAddOrderRequestBodyDto body : unityOrderRequestDto.getFurnitures()){
            OrderableFurniture orderableFurniture = new OrderableFurniture();
            orderableFurniture.setFurniture(furnitureService.getFurnitureForUnityById(body.getId()));
            orderableFurniture.setTransform(body.getPosition().getX(), body.getPosition().getY(), body.getPosition().getZ(), body.getRotation().getX(), body.getRotation().getY(), body.getRotation().getZ());
            orderedFurnitures.add(orderableFurniture);
        }
        return orderedFurnitures;
    }
    @Override
    public UnityCustomerAuthenticationResponseDto authenticate(UnityCustomerAuthenticationRequestDto userRequestDto){
        List<Customer> customers = customerService.getCustomersForUnity();
        StringBuilder username = new StringBuilder(userRequestDto.getCustomer().getUsername());
        StringBuilder password = new StringBuilder(userRequestDto.getCustomer().getPassword());
        Boolean authenticate = Boolean.FALSE;
        Long userId = (long) -1;
        for(Customer customer : customers){
            if(customer.getUsername().contentEquals(username.toString()) && customer.getPassword().contentEquals(password.toString())){
                authenticate = true;
                userId = customer.getId();
            }
        }
        String response = authenticate.toString();
        return new UnityCustomerAuthenticationResponseDto(response, userId);
    }
    //WebGL
    @Override
    public UnityGetOrderResponseDto getWebGLById(Long id) {
        Order requestedOrder = orderService.getOrderForUnityWebGL(id);
        UnityGetOrderResponseDto unityWebGLGetOrderResponseDto = new UnityGetOrderResponseDto();
        List<UnityGetOrderResponseBodyDto> bodies = new ArrayList<>();
        if (requestedOrder != null) {
            for (OrderableFurniture orderableFurniture : requestedOrder.getFurnitures()) {
                UnityGetOrderResponseBodyDto unityOrderResponseBodyDto = getUnityGetOrderResponseBodyDto(orderableFurniture);
                bodies.add(unityOrderResponseBodyDto);
            }
        }
        unityWebGLGetOrderResponseDto.setFurnitures(bodies);
        return unityWebGLGetOrderResponseDto;
    }
    @Override
    public UnityGetOrderResponseBodyDto getUnityGetOrderResponseBodyDto(OrderableFurniture orderableFurniture) {
        UnityGetOrderResponseBodyDto unityOrderResponseBodyDto = new UnityGetOrderResponseBodyDto();
        unityOrderResponseBodyDto.setId(orderableFurniture.getFurniture().getId());
        unityOrderResponseBodyDto.setPosition(new Vector3(orderableFurniture.getPosX(), orderableFurniture.getPosY(), orderableFurniture.getPosZ()));
        unityOrderResponseBodyDto.setRotation(new Vector3(orderableFurniture.getRotX(), orderableFurniture.getRotY(), orderableFurniture.getRotZ()));
        return unityOrderResponseBodyDto;
    }
    //WebGL App routing methods.
    @Override
    public byte[] buildTheWebGLApp(String folderName, String fileName) throws IOException {
        String path = "/" + folderName + "/" + fileName;
        Resource resource = new ClassPathResource("static" + path);
        return Files.readAllBytes(resource.getFile().toPath());
    }
    @Override
    public byte[] buildTheWebGLApp(String fileName) throws IOException {
        String path = "/" + fileName;
        Resource resource = new ClassPathResource("static" + path);
        return Files.readAllBytes(resource.getFile().toPath());
    }
    @Override
    public byte[] buildTheWebGLApp(Long id, String folderName, String fileName) throws IOException {
        String path = "/" + folderName + "/" + fileName;
        Resource resource = new ClassPathResource("static" + path);
        return Files.readAllBytes(resource.getFile().toPath());
    }
}
