package com.haiykut.ardecorifywebapi.model;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.RequiredArgsConstructor;
import lombok.Setter;
import java.util.List;
@Entity(name = "orderr")
@Getter
@Setter
@RequiredArgsConstructor
public class Order {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long orderId;
    @ManyToOne
    private Customer orderedBy;
    @OneToMany(cascade = {CascadeType.PERSIST, CascadeType.REMOVE})
    private List<OrderableFurniture> furnitures;
}
