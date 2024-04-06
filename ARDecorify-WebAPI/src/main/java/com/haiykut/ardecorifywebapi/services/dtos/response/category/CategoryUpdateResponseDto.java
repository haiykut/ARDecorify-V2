package com.haiykut.ardecorifywebapi.services.dtos.response.category;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class CategoryUpdateResponseDto {
    private Long id;
    private String name;
}
