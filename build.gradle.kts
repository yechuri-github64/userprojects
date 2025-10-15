plugins {
    id("java")
    id("application")
    id("io.micronaut.application") version "4.3.4"
    id("io.micronaut.aot") version "4.3.4"
    id("com.github.johnrengelman.shadow") version "8.1.1"
}

group = "com.ai2dev.test_project_java"
version = "0.1"
java.sourceCompatibility = JavaVersion.VERSION_17

repositories {
    mavenCentral()
}

dependencies {
    implementation(platform("io.micronaut:micronaut-bom:3.9.7"))

    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut:micronaut-http-client")
    implementation("io.micronaut.data:micronaut-data-jdbc")
    implementation("io.micronaut.sql:micronaut-jdbc-hikari")
    implementation("io.micronaut:micronaut-validation")
    implementation("io.micronaut:micronaut-http-server-netty")
    implementation("io.micronaut.inject:micronaut-inject-java")

    implementation("org.yaml:snakeyaml:2.0")
    implementation("mysql:mysql-connector-java:8.1.0")

    annotationProcessor("io.micronaut:micronaut-inject-java")
    annotationProcessor("io.micronaut.data:micronaut-data-processor")

    runtimeOnly("ch.qos.logback:logback-classic:1.4.11")
}

application {
    mainClass.set("com.ai2dev.test_project_java.Application")
}

micronaut {
    runtime("netty")
    processing {
        incremental(true)
        annotations("com.ai2dev.test_project_java.*")
    }
}
