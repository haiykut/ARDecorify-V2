package com.haiykut.ardecorifywebapi.services.dtos.request.furniture;

import jakarta.validation.constraints.NotEmpty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class FurnitureUpdateRequestDto {
    @NotEmpty
    private String name;
    @NotEmpty
    private Long categoryId;
}
