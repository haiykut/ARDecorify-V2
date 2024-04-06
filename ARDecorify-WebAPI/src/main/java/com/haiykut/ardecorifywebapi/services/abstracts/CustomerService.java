package com.haiykut.ardecorifywebapi.services.abstracts;
import com.haiykut.ardecorifywebapi.entities.Customer;
import com.haiykut.ardecorifywebapi.services.dtos.request.customer.CustomerAddRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.customer.CustomerGetRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.customer.CustomerUpdateRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.customer.CustomerAddResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.customer.CustomerGetResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.customer.CustomerUpdateResponseDto;

import java.util.List;
public interface CustomerService {
    List<CustomerGetResponseDto> getCustomers();
    CustomerGetResponseDto getCustomerById(Long id);
    Customer getCustomerByIdForUnity(Long id);
    CustomerAddResponseDto register(CustomerAddRequestDto userRequestDto);
    void deleteCustomerById(Long id);
    void deleteCustomers();
    CustomerUpdateResponseDto updateCustomerById(Long id, CustomerUpdateRequestDto userRequestDto);
    List<Customer> getCustomersForUnity();
}
