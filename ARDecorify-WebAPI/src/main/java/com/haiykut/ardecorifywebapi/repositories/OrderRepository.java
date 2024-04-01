package com.haiykut.ardecorifywebapi.repositories;
import com.haiykut.ardecorifywebapi.entities.Order;
import org.springframework.data.jpa.repository.JpaRepository;
public interface OrderRepository extends JpaRepository<Order, Long> {
}
