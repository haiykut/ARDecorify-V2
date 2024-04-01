package com.haiykut.ardecorifywebapi.repositories;
import com.haiykut.ardecorifywebapi.entities.Furniture;
import org.springframework.data.jpa.repository.JpaRepository;
public interface FurnitureRepository extends JpaRepository<Furniture, Long> {
}
