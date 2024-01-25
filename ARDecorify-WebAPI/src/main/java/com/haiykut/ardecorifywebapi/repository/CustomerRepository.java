package com.haiykut.ardecorifywebapi.repository;
import com.haiykut.ardecorifywebapi.model.Customer;
import org.springframework.data.jpa.repository.JpaRepository;
public interface CustomerRepository extends JpaRepository<Customer, Long> {
}
