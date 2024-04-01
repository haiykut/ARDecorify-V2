package com.haiykut.ardecorifywebapi.services.dtos.response;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor
public class OrderableFurnitureResponseDto {
    private Long id;
    private String categoryName;
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
