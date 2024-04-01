package com.haiykut.ardecorifywebapi.services.dtos.request.unity;
import com.haiykut.ardecorifywebapi.services.dtos.Vector3;
import lombok.Data;
@Data
public class UnityAddOrderRequestBodyDto {
    private Long id;
    private Vector3 position;
    private Vector3 rotation;
}
