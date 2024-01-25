package com.haiykut.ardecorifywebapi.model;

import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import jakarta.persistence.ManyToOne;
import lombok.Getter;
import lombok.RequiredArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@RequiredArgsConstructor
@Entity
public class OrderableFurniture {
    @Id
    @GeneratedValue
    private Long orderableFurnitureId;
    @ManyToOne
    private Furniture furniture;
    private float posX, posY, posZ, rotX, rotY, rotZ;
    public void setTransform(float posX, float posY, float posZ, float rotX, float rotY, float rotZ){
        this.posX = posX;
        this.posY = posY;
        this.posZ = posZ;
        this.rotX = rotX;
        this.rotY = rotY;
        this.rotZ = rotZ;
    }
}
