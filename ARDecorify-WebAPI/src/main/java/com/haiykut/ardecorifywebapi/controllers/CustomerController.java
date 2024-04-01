package com.haiykut.ardecorifywebapi.controllers;
import com.haiykut.ardecorifywebapi.services.abstracts.CustomerService;
import com.haiykut.ardecorifywebapi.services.dtos.request.CustomerRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.CustomerResponseDto;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import java.util.List;
@RestController
@RequestMapping("/api/customer")
@RequiredArgsConstructor
public class CustomerController {
    private final CustomerService customerService;
    @GetMapping
    public ResponseEntity<List<CustomerResponseDto>> getCustomers(){
        return ResponseEntity.ok(customerService.getCustomers());
    }
    @GetMapping("/{id}")
    public ResponseEntity<CustomerResponseDto> getCustomerById(@PathVariable Long id){
        return ResponseEntity.ok(customerService.getCustomerById(id));
    }
    @PostMapping("/register")
    public ResponseEntity<CustomerResponseDto> register(@RequestBody CustomerRequestDto customerRequestDto){
        return ResponseEntity.ok(customerService.register(customerRequestDto));
    }
    @DeleteMapping("/delete/{id}")
    public ResponseEntity<String> deleteCustomerById(@PathVariable Long id){
        customerService.deleteCustomerById(id);
        return new ResponseEntity<>("Customer Deleted!", HttpStatus.OK);
    }
    @DeleteMapping("/delete/all")
    public ResponseEntity<String> deleteCustomers(){
        customerService.deleteCustomers();
        return new ResponseEntity<>("Customers Deleted!", HttpStatus.OK);
    }
    @PutMapping("/update/{id}")
    public ResponseEntity<CustomerResponseDto> updateCustomerById(@PathVariable Long id, @RequestBody CustomerRequestDto customerRequestDto){
        return ResponseEntity.ok(customerService.updateCustomerById(id, customerRequestDto));
    }
}
