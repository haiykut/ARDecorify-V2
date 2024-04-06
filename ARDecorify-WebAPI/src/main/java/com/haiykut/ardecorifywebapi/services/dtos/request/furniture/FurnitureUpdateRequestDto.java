package com.haiykut.ardecorifywebapi.services.dtos.request.furniture;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotEmpty;
import jakarta.validation.constraints.Size;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class FurnitureUpdateRequestDto {
    @NotBlank
    @Size(min = 3, max = 50)
    private String name;
    @NotBlank
    private Long categoryId;
}
