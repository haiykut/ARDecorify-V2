package com.haiykut.ardecorifywebapi.dto.response;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class FurnitureResponseDto {
    private Long id;
    private String name;
    private Long categoryId;
}
