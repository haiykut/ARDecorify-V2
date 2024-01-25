package com.haiykut.ardecorifywebapi.repository;

import com.haiykut.ardecorifywebapi.model.Order;
import org.springframework.data.jpa.repository.JpaRepository;

public interface OrderRepository extends JpaRepository<Order, Long> {
}
