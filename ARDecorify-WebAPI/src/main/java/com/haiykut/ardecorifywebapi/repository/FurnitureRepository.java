package com.haiykut.ardecorifywebapi.repository;
import com.haiykut.ardecorifywebapi.model.Furniture;
import org.springframework.data.jpa.repository.JpaRepository;
public interface FurnitureRepository extends JpaRepository<Furniture, Long> {
}
