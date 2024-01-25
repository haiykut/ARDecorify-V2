package com.haiykut.ardecorifywebapi.service;
import com.haiykut.ardecorifywebapi.configuration.MapperConfig;
import com.haiykut.ardecorifywebapi.dto.request.CustomerRequestDto;
import com.haiykut.ardecorifywebapi.dto.response.CustomerResponseDto;
import com.haiykut.ardecorifywebapi.model.Customer;
import com.haiykut.ardecorifywebapi.repository.CustomerRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.List;
import java.util.stream.Collectors;
@Service
@RequiredArgsConstructor
public class CustomerService {
    private final CustomerRepository customerRepository;
    private final MapperConfig mapperConfig;
    public List<CustomerResponseDto> getCustomers(){
        return customerRepository.findAll().stream().map(user -> mapperConfig.modelMapper().map(user, CustomerResponseDto.class)).collect(Collectors.toList());
    }
    public CustomerResponseDto getCustomerById(Long id){
        return mapperConfig.modelMapper().map(customerRepository.findById(id).orElseThrow(), CustomerResponseDto.class);
    }
    public Customer getCustomerByIdForUnity(Long id){
        return customerRepository.findById(id).orElseThrow();
    }
    public CustomerResponseDto addCustomer(CustomerRequestDto userRequestDto){
        Customer newUser = new Customer();
        newUser.setUsername(userRequestDto.getUsername());
        newUser.setPassword(userRequestDto.getPassword());
        customerRepository.save(newUser);
        return mapperConfig.modelMapper().map(newUser, CustomerResponseDto.class);
    }
    public void deleteCustomerById(Long id){
        Customer requestedUser = customerRepository.findById(id).orElseThrow();
        customerRepository.delete(requestedUser);
    }
    public void deleteAllCustomers(){
        customerRepository.deleteAll();
    }
    public CustomerResponseDto updateCustomer(Long id, CustomerRequestDto userRequestDto){
        Customer requestedUser = customerRepository.findById(id).orElseThrow();
        requestedUser.setUsername(userRequestDto.getUsername());
        requestedUser.setPassword(userRequestDto.getPassword());
        return mapperConfig.modelMapper().map(requestedUser, CustomerResponseDto.class);
    }
    public List<Customer> getCustomersForUnity(){
        return customerRepository.findAll();
    }

}
