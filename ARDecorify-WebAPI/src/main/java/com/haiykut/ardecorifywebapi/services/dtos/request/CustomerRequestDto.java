package com.haiykut.ardecorifywebapi.services.dtos.request;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class CustomerRequestDto {
    private String username;
    private String password;
}
