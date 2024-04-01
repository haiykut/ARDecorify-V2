package com.haiykut.ardecorifywebapi.services.abstracts;
import com.haiykut.ardecorifywebapi.entities.Customer;
import com.haiykut.ardecorifywebapi.services.dtos.request.CustomerRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.CustomerResponseDto;
import java.util.List;
public interface CustomerService {
    List<CustomerResponseDto> getCustomers();
    CustomerResponseDto getCustomerById(Long id);
    Customer getCustomerByIdForUnity(Long id);
    CustomerResponseDto register(CustomerRequestDto userRequestDto);
    void deleteCustomerById(Long id);
    void deleteCustomers();
    CustomerResponseDto updateCustomerById(Long id, CustomerRequestDto userRequestDto);
    List<Customer> getCustomersForUnity();
}
