package com.haiykut.ardecorifywebapi.dto.response.unity.webgl;
import com.haiykut.ardecorifywebapi.dto.Vector3;
import lombok.Data;
@Data
public class UnityGetOrderResponseBodyDto {
    private Long id;
    private Vector3 position;
    private Vector3 rotation;
}
