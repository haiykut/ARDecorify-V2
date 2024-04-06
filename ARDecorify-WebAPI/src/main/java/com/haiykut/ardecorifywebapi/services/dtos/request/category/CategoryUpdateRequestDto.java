package com.haiykut.ardecorifywebapi.services.dtos.request.category;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class CategoryUpdateRequestDto {
    @NotBlank
    @Size(min = 2, max = 50)
    private String name;
}
