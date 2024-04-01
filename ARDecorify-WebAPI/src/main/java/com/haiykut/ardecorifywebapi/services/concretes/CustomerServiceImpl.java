package com.haiykut.ardecorifywebapi.services.concretes;
import com.haiykut.ardecorifywebapi.configuration.MapperConfig;
import com.haiykut.ardecorifywebapi.services.abstracts.CustomerService;
import com.haiykut.ardecorifywebapi.services.dtos.request.CustomerRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.CustomerResponseDto;
import com.haiykut.ardecorifywebapi.entities.Customer;
import com.haiykut.ardecorifywebapi.repositories.CustomerRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.util.List;
import java.util.stream.Collectors;
@Service
@RequiredArgsConstructor
public class CustomerServiceImpl implements CustomerService {
    private final CustomerRepository customerRepository;
    private final MapperConfig mapperConfig;
    @Override
    public List<CustomerResponseDto> getCustomers(){
        return customerRepository.findAll().stream().map(user -> mapperConfig.modelMapper().map(user, CustomerResponseDto.class)).collect(Collectors.toList());
    }
    @Override
    public CustomerResponseDto getCustomerById(Long id){
        return mapperConfig.modelMapper().map(customerRepository.findById(id).orElseThrow(), CustomerResponseDto.class);
    }
    @Override
    public Customer getCustomerByIdForUnity(Long id){
        return customerRepository.findById(id).orElseThrow();
    }
    @Override
    public CustomerResponseDto register(CustomerRequestDto userRequestDto){
        Customer newUser = new Customer();
        newUser.setUsername(userRequestDto.getUsername());
        newUser.setPassword(userRequestDto.getPassword());
        customerRepository.save(newUser);
        return mapperConfig.modelMapper().map(newUser, CustomerResponseDto.class);
    }
    @Override
    public void deleteCustomerById(Long id){
        Customer requestedUser = customerRepository.findById(id).orElseThrow();
        customerRepository.delete(requestedUser);
    }
    @Override
    public void deleteCustomers(){
        customerRepository.deleteAll();
    }
    @Override
    public CustomerResponseDto updateCustomerById(Long id, CustomerRequestDto userRequestDto){
        Customer requestedUser = customerRepository.findById(id).orElseThrow();
        requestedUser.setUsername(userRequestDto.getUsername());
        requestedUser.setPassword(userRequestDto.getPassword());
        return mapperConfig.modelMapper().map(requestedUser, CustomerResponseDto.class);
    }
    @Override
    public List<Customer> getCustomersForUnity(){
        return customerRepository.findAll();
    }
}
