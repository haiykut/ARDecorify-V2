package com.haiykut.ardecorifywebapi.services.dtos.response.customer;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class CustomerGetResponseDto {
    private Long customerId;
    private String username;
    private String password;
}
