FROM gradle:8.7.0-jdk21 AS build
WORKDIR /home/gradle/project
COPY . .
RUN gradle clean shadowJar --no-daemon

FROM eclipse-temurin:21-jre
WORKDIR /app
COPY --from=build /home/gradle/project/build/libs/*-all.jar /app/app.jar
EXPOSE 8080
ENTRYPOINT ["java","-jar","/app/app.jar"]
