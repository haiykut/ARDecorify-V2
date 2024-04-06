package com.haiykut.ardecorifywebapi.services.concretes;
import com.haiykut.ardecorifywebapi.configurations.MapperConfig;
import com.haiykut.ardecorifywebapi.services.abstracts.CustomerService;
import com.haiykut.ardecorifywebapi.services.dtos.request.customer.CustomerAddRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.customer.CustomerGetRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.request.customer.CustomerUpdateRequestDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.customer.CustomerAddResponseDto;
import com.haiykut.ardecorifywebapi.services.dtos.response.customer.CustomerGetResponseDto;
import com.haiykut.ardecorifywebapi.entities.Customer;
import com.haiykut.ardecorifywebapi.repositories.CustomerRepository;
import com.haiykut.ardecorifywebapi.services.dtos.response.customer.CustomerUpdateResponseDto;
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
    public List<CustomerGetResponseDto> getCustomers(){
        return customerRepository.findAll().stream().map(user -> mapperConfig.modelMapper().map(user, CustomerGetResponseDto.class)).collect(Collectors.toList());
    }
    @Override
    public CustomerGetResponseDto getCustomerById(Long id){
        return mapperConfig.modelMapper().map(customerRepository.findById(id).orElseThrow(), CustomerGetResponseDto.class);
    }
    @Override
    public Customer getCustomerByIdForUnity(Long id){
        return customerRepository.findById(id).orElseThrow();
    }
    @Override
    public CustomerAddResponseDto register(CustomerAddRequestDto userRequestDto){
        Customer newUser = new Customer();
        newUser.setUsername(userRequestDto.getUsername());
        newUser.setPassword(userRequestDto.getPassword());
        customerRepository.save(newUser);
        return mapperConfig.modelMapper().map(newUser, CustomerAddResponseDto.class);
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
    public CustomerUpdateResponseDto updateCustomerById(Long id, CustomerUpdateRequestDto userRequestDto){
        Customer requestedUser = customerRepository.findById(id).orElseThrow();
        requestedUser.setUsername(userRequestDto.getUsername());
        requestedUser.setPassword(userRequestDto.getPassword());
        return mapperConfig.modelMapper().map(requestedUser, CustomerUpdateResponseDto.class);
    }
    @Override
    public List<Customer> getCustomersForUnity(){
        return customerRepository.findAll();
    }
}
