package com.haiykut.ardecorifywebapi.services.dtos.request.customer;
import jakarta.validation.constraints.NotEmpty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class CustomerGetRequestDto {
    private String username;
    private String password;
}
