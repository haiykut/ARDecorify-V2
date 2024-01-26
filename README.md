
# 🚀ARDecorify V2

The purpose of this project is to enable interaction between designers/dealers and customers without any physical necessity by using augmented reality technologies.

## 🏗️Architecture

The architecture of the project and this repository consists of folders named "Mobile" and "WebGL" made with Unity and "WebAPI" made with Java Spring Boot. Each folder is a source file for different and related application.

- In the [Mobile section](https://github.com/haiykut/ARDecorify-V2/tree/main/ARDecorify-Mobile), EasyAR SDK is utilized for surface scanning and image target features, allowing customers to place furnitures in empty spaces such as rooms to get a preliminary idea. Also customers can send these decoration designs to designers/dealers.

- In the [WebGL section](https://github.com/haiykut/ARDecorify-V2/tree/main/ARDecorify-WebGL), designers/dealers can view designs submitted by customers in a virtual room. The goal is to provide feedback to customers and enhance the decoration.

- In the [WebAPI section](https://github.com/haiykut/ARDecorify-V2/tree/main/ARDecorify-WebAPI), facilitates communication between customers and designers/dealers, laying the foundation for future features to be added, further advancing the project.

## 🛠️Installation & Run

- For the Mobile app, you should open the mobile section in the Unity editor, set your [EasyAR SDK](https://portal.easyar.com/) Key and relevant url settings in editor and build the app.
  After you got your build file(.apk), install it to an Android device that has gyroscope and accelerometer sensors.

- For the WebGL app, you must set the necessary url adjustments in the Unity editor and build the app.
   After you got your build files;
 Move Build, TemplateData folders and logo.png file to WebAPI/src/main/resources/static directory and move index.html to WebAPI/src/main/resources/templates directory.

- To run your WebAPI app, you can use [Java Spring Boot's instructions](https://spring.io/guides/gs/spring-boot/).

## 🦾Technologies

- [Unity](https://unity.com/learn/get-started)
- [EasyAR](https://help.easyar.com/EasyAR%20Sense/v1/Getting%20Started/Getting-Started-with-EasyAR.html) 
- [Java 21](https://docs.oracle.com/en/java/javase/21/migrate/getting-started.html#GUID-C25E2B1D-6C24-4403-8540-CFEA875B994A)
- [Spring Boot 3.2.2](https://docs.spring.io/spring-boot/docs/current/reference/html/getting-started.html)
- [Maven](https://maven.apache.org/guides/getting-started/)
- [Lombok](https://projectlombok.org/setup/)
- [ModelMapper](https://modelmapper.org/getting-started/)
- [SwaggerUI](https://swagger.io/tools/open-source/getting-started/)
- [PostgreSQL](https://www.postgresql.org/docs/)
- Restful API
  
## 🖼️Screenshots

![Mobile App](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/mobile.jpg)
![WebGL App](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/webgl.jpg)
![WebAPI Entity Relations](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/entityrelations.jpg)
![PostgreSQL DB](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/db.jpg)
![OrderableFurniture DB table](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/db2.jpg)
![Furniture DB table](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/db3.jpg)
![WebAPI Swagger Sample](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/swagger.jpg)
![WebAPI Swagger Sample 2](https://github.com/haiykut/ARDecorify-V2/blob/main/screenshots/swagger2.jpg)

