package com.haiykut.ardecorifywebapi.dto.request.unity;
import com.haiykut.ardecorifywebapi.util.Vector3;
import lombok.Data;
@Data
public class UnityAddOrderRequestBodyDto {
    private Long id;
    private Vector3 position;
    private Vector3 rotation;
}
