package com.haiykut.ardecorifywebapi.controller;

import com.haiykut.ardecorifywebapi.dto.request.unity.UnityAddOrderRequestDto;
import com.haiykut.ardecorifywebapi.dto.request.unity.UnityCustomerAuthenticationRequestDto;
import com.haiykut.ardecorifywebapi.dto.response.unity.mobile.UnityAddOrderResponseDto;
import com.haiykut.ardecorifywebapi.dto.response.unity.mobile.UnityCustomerAuthenticationResponseDto;
import com.haiykut.ardecorifywebapi.service.UnityService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;

@Controller
@RequestMapping("/api/unity")
@RequiredArgsConstructor
public class UnityController {
    private final UnityService unityService;
    @ResponseBody
    @PostMapping("/mobile/addOrder")
    public ResponseEntity<UnityAddOrderResponseDto> addOrder(@RequestBody UnityAddOrderRequestDto unityOrderRequestDto){
        return ResponseEntity.ok(unityService.addOrder(unityOrderRequestDto));
    }
    @ResponseBody
    @PostMapping("/mobile/authenticate")
    public ResponseEntity<UnityCustomerAuthenticationResponseDto> UnityAuthenticate(@RequestBody UnityCustomerAuthenticationRequestDto unityUserRequestDto){
        return ResponseEntity.ok(unityService.authenticateUnity(unityUserRequestDto));
    }
}
