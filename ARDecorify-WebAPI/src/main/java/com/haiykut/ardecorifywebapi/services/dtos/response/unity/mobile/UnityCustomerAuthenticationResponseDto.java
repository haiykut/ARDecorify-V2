package com.haiykut.ardecorifywebapi.services.dtos.response.unity.mobile;
import lombok.*;
@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor
public class UnityCustomerAuthenticationResponseDto {
    private String sessionMessage;
    private Long userId;
}
