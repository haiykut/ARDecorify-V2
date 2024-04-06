package com.haiykut.ardecorifywebapi.services.dtos.response.furniture;

import jakarta.validation.constraints.NotEmpty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class FurnitureAddResponseDto {
    @NotEmpty
    private String name;
    @NotEmpty
    private Long categoryId;
}
