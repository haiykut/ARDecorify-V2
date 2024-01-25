package com.haiykut.ardecorifywebapi.controller;

import com.haiykut.ardecorifywebapi.dto.request.CustomerRequestDto;
import com.haiykut.ardecorifywebapi.dto.response.CustomerResponseDto;
import com.haiykut.ardecorifywebapi.service.CustomerService;
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
    @GetMapping()
    public ResponseEntity<List<CustomerResponseDto>> getAllCustomers(){
        return ResponseEntity.ok(customerService.getCustomers());
    }
    @GetMapping("/{id}")
    public ResponseEntity<CustomerResponseDto> getCustomerById(@PathVariable Long id){
        return ResponseEntity.ok(customerService.getCustomerById(id));
    }
    @PostMapping("/register")
    public ResponseEntity<CustomerResponseDto> register(@RequestBody CustomerRequestDto customerRequestDto){
        return ResponseEntity.ok(customerService.addCustomer(customerRequestDto));
    }
    @DeleteMapping("/delete/{id}")
    public ResponseEntity<String> deleteCustomerById(@PathVariable Long id){
        customerService.deleteCustomerById(id);
        return new ResponseEntity<>("Customer Deleted!", HttpStatus.OK);
    }
    @DeleteMapping("/delete/all")
    public ResponseEntity<String> deleteAllCustomers(){
        customerService.deleteAllCustomers();
        return new ResponseEntity<>("Customers Deleted!", HttpStatus.OK );
    }
    @PutMapping("/update/{id}")
    public ResponseEntity<CustomerResponseDto> updateCustomerById(@PathVariable Long id, @RequestBody CustomerRequestDto customerRequestDto){
        return ResponseEntity.ok(customerService.updateCustomer(id, customerRequestDto));
    }
}
