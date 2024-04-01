package com.haiykut.ardecorifywebapi.services.dtos.response.unity.webgl;
import lombok.Data;
import java.util.List;
@Data
public class UnityGetOrderResponseDto {
    private List<UnityGetOrderResponseBodyDto> furnitures;
}
