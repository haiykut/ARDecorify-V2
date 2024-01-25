package com.haiykut.ardecorifywebapi.dto.request.unity;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Setter;

import java.util.List;
@Getter
@Setter
@AllArgsConstructor
public class UnityAddOrderRequestDto {
    private Long orderedBy;
    private List<UnityAddOrderRequestBodyDto> furnitures;
}
