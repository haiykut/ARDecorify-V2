package com.haiykut.ardecorifywebapi.services.dtos.response.customer;

import jakarta.validation.constraints.NotEmpty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class CustomerAddResponseDto {
    @NotEmpty
    private String username;
    @NotEmpty
    private String password;
}
