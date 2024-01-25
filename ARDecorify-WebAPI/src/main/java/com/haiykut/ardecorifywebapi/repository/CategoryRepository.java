package com.haiykut.ardecorifywebapi.repository;

import com.haiykut.ardecorifywebapi.model.Category;
import org.springframework.data.jpa.repository.JpaRepository;

public interface CategoryRepository extends JpaRepository<Category, Long> {
}
