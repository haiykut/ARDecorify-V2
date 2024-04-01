package com.haiykut.ardecorifywebapi.model;
import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.RequiredArgsConstructor;
import lombok.Setter;

import java.util.List;
@Entity
@Setter
@Getter
@RequiredArgsConstructor
public class Category {
    @Id
    @GeneratedValue
    @JsonIgnore
    private Long id;
    private String name;
    @OneToMany(mappedBy = "category", cascade = CascadeType.ALL, fetch = FetchType.LAZY)
    private List<Furniture> furnitures;
}
