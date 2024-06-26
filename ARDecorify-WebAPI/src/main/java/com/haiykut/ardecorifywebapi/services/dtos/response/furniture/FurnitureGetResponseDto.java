package com.haiykut.ardecorifywebapi.services.dtos.response.furniture;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class FurnitureGetResponseDto {
    private Long id;
    private String name;
    private Long categoryId;
}
