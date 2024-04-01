package com.haiykut.ardecorifywebapi.controllers;
import com.haiykut.ardecorifywebapi.services.abstracts.UnityService;
import com.haiykut.ardecorifywebapi.services.dtos.request.unity.UnityAddOrderRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.unity.UnityCustomerAuthenticationRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.unity.mobile.UnityAddOrderResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.unity.mobile.UnityCustomerAuthenticationResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.unity.webgl.UnityGetOrderResponseDto;
import io.swagger.v3.oas.annotations.Hidden;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.*;
import java.io.IOException;
@Controller
@RequestMapping("/api/unity")
@RequiredArgsConstructor
public class UnityController {
    private final UnityService unityService;
    //Mobile
    @ResponseBody
    @PostMapping("/mobile/addOrder")
    public ResponseEntity<UnityAddOrderResponseDto> addOrder(@RequestBody UnityAddOrderRequestDto unityOrderRequestDto){
        return ResponseEntity.ok(unityService.addOrder(unityOrderRequestDto));
    }
    @ResponseBody
    @PostMapping("/mobile/authenticate")
    public ResponseEntity<UnityCustomerAuthenticationResponseDto> authenticate(@RequestBody UnityCustomerAuthenticationRequestDto unityUserRequestDto){
        return ResponseEntity.ok(unityService.authenticate(unityUserRequestDto));
    }
    //WebGL
    @GetMapping("/webgl/{id}")
    @ResponseBody
    public ResponseEntity<UnityGetOrderResponseDto> unityWebGLById(@PathVariable Long id){
        return ResponseEntity.ok(unityService.getWebGLById(id));
    }
    @GetMapping("/webgl/show/{id}")
    public String showWebGL(){
        return "index";
    }
    @GetMapping({"/webgl/show/{folderName}/{fileName}", "/webgl/static/{folderName}/{fileName}"})
    @ResponseBody
    @Hidden
    public ResponseEntity<byte[]> buildTheApp(@PathVariable String folderName, @PathVariable String fileName) throws IOException {
        return ResponseEntity.ok(unityService.buildTheWebGLApp(folderName, fileName));
    }
    @GetMapping("/webgl/static/{fileName}")
    @ResponseBody
    @Hidden
    public ResponseEntity<byte[]> buildTheApp(@PathVariable String fileName) throws IOException {
        return ResponseEntity.ok(unityService.buildTheWebGLApp(fileName));
    }
    @GetMapping("/webgl/show/{id}/static/{folderName}/{fileName}")
    @ResponseBody
    @Hidden
    public ResponseEntity<byte[]> buildTheApp(@PathVariable Long id, @PathVariable String folderName, @PathVariable String fileName) throws IOException {
        return ResponseEntity.ok(unityService.buildTheWebGLApp(id, folderName, fileName));
    }
}
