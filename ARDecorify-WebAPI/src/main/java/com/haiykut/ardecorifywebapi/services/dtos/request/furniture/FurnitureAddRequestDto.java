package com.haiykut.ardecorifywebapi.services.dtos.request.furniture;

import jakarta.validation.constraints.NotEmpty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Setter;

@AllArgsConstructor
@Getter
@Setter
public class FurnitureAddRequestDto {
    @NotEmpty
    private String name;
    @NotEmpty
    private Long categoryId;
}
