package com.haiykut.ardecorifywebapi.services.dtos.request.unity;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Setter;
@AllArgsConstructor
@Getter
@Setter
public class UnityCustomerAuthenticationRequestBodyDto {
    private String username;
    private String password;
}
