
FROM maven:3.8.3-openjdk-17 as  build
WORKDIR /app
COPY . /app/
RUN mvn clean package -DskipTests

FROM openjdk:17-jdk
COPY --from=build /app/target/ARDecorify-WebAPI-0.0.1-SNAPSHOT.jar /app/ardecorify.jar
EXPOSE 8080
CMD java -jar /app/ardecorify.jar


