
# 🚀ARDecorify V2

The purpose of this project is to enable interaction between designers/dealers and customers without any physical necessity by using augmented reality technologies.

## 🏗️Architecture

The architecture of the project and this repository consists of folders named "Mobile" and "WebGL" made with Unity and "WebAPI" made with Java Spring Boot. Each folder is a source file for an application.

- In the [Mobile section](https://github.com/haiykut/ARDecorify-V2/tree/main/ARDecorify-Mobile), EasyAR SDK is utilized for plane detection and image target features, allowing customers to place furnitures in empty spaces such as rooms to get a preliminary idea. Also customers can send these decoration designs to designers/dealers then.

- In the [WebGL section](https://github.com/haiykut/ARDecorify-V2/tree/main/ARDecorify-WebGL), designers/dealers can view designs submitted by customers in a virtual room. The goal is to provide feedback to customers and enhance the decoration.

- In the [WebAPI section](https://github.com/haiykut/ARDecorify-V2/tree/main/ARDecorify-WebAPI), facilitates communication between customers and designers/dealers, laying the foundation for future features to be added, further advancing the project.

## 🛠️Installation & Run

- For the Mobile app, you should open the mobile section in the Unity editor, set your [EasyAR SDK](https://www.easyar.com/) Key and relevant url settings in editor and get a build.
After you got your build, install .apk build file to an Android device that has gyroscope and accelerometer sensors.

- For the WebGL app, you must set the necessary url adjustments in the Unity editor and get the build. 
After you got your build files;
 Move Build, TemplateData folders and logo.png file to WebAPI/src/main/resources/static directory and move index.html to WebAPI/src/main/resources/templates directory.

- To run your WebAPI app, you can use [Java Spring Boot's instructions](https://spring.io/guides/gs/spring-boot/).

## 🦾Technologies
- Unity
- EasyAR 
- Java 21
- Spring Boot 3.2.2
- Maven
- Lombok
- Thymeleaf
- ModelMapper
- Restful API
- SwaggerUI
- PostgreSQL

## 🖼️Screenshots
![Mobile App](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/mobile.jpg)
![WebGL App](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/webgl.jpg)
![WebAPI Entity Relations](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/entityrelations.jpg)
![PostgreSQL DB](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/db.jpg)
![WebAPI Swagger Sample](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/swagger.jpg)
![WebAPI Swagger Sample 2](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/swagger2.jpg)
![OrderableFurniture DB table](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/db2.jpg)
![Furniture DB table](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/db3.jpg)

