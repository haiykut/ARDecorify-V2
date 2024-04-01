package com.haiykut.ardecorifywebapi.repositories;
import com.haiykut.ardecorifywebapi.entities.Category;
import org.springframework.data.jpa.repository.JpaRepository;
public interface CategoryRepository extends JpaRepository<Category, Long> {
}
