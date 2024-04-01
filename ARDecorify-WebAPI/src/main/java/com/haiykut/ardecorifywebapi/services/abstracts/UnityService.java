package com.haiykut.ardecorifywebapi.services.abstracts;
import com.haiykut.ardecorifywebapi.entities.OrderableFurniture;
import com.haiykut.ardecorifywebapi.services.dtos.request.unity.UnityAddOrderRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.unity.UnityCustomerAuthenticationRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.unity.mobile.UnityAddOrderResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.unity.mobile.UnityCustomerAuthenticationResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.unity.webgl.UnityGetOrderResponseBodyDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.unity.webgl.UnityGetOrderResponseDto;
import java.io.IOException;
import java.util.List;
public interface UnityService {
    UnityAddOrderResponseDto addOrder(UnityAddOrderRequestDto unityOrderRequestDto);
    List<OrderableFurniture> getOrderableFurnitures(UnityAddOrderRequestDto unityOrderRequestDto);
    UnityCustomerAuthenticationResponseDto authenticate(UnityCustomerAuthenticationRequestDto userRequestDto);
    UnityGetOrderResponseDto getWebGLById(Long id);
    UnityGetOrderResponseBodyDto getUnityGetOrderResponseBodyDto(OrderableFurniture orderableFurniture);
    byte[] buildTheWebGLApp(String folderName, String fileName) throws IOException;
    byte[] buildTheWebGLApp(String fileName) throws IOException;
    byte[] buildTheWebGLApp(Long id, String folderName, String fileName) throws IOException;
}
