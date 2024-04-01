package com.haiykut.ardecorifywebapi.services.dtos.response;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.List;
@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class OrderResponseDto {
    private Long id;
    private Long orderedBy;
    private List<OrderableFurnitureResponseDto> furnitures;
}
