package com.haiykut.ardecorifywebapi.dto.request;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Setter;
@AllArgsConstructor
@Getter
@Setter
public class FurnitureRequestDto {
    private String name;
    private Long categoryId;
}
