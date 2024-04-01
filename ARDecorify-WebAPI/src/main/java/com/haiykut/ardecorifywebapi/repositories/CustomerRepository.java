package com.haiykut.ardecorifywebapi.repositories;
import com.haiykut.ardecorifywebapi.entities.Customer;
import org.springframework.data.jpa.repository.JpaRepository;
public interface CustomerRepository extends JpaRepository<Customer, Long> {
}
