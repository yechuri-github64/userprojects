plugins {
    id("io.micronaut.application") version "4.3.4"
    id("io.micronaut.aot") version "4.3.4"
    id("com.github.johnrengelman.shadow") version "8.1.1"
    id("java")
}

group = "com.ai2dev"
version = "0.1"

java {
    sourceCompatibility = JavaVersion.VERSION_17
    targetCompatibility = JavaVersion.VERSION_17
}

repositories {
    mavenCentral()
}

dependencies {
    implementation(platform("io.micronaut:micronaut-bom:3.9.7"))
    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut:micronaut-http-client")
    implementation("io.micronaut:micronaut-http-server-netty")
    implementation("io.micronaut:micronaut-validation")
    implementation("io.micronaut:micronaut-inject")
    implementation("org.yaml:snakeyaml:2.2")
    // Backend connectors with explicit versions
    implementation("org.postgresql:postgresql:42.5.1")
    implementation("org.mongodb:mongodb-driver-sync:4.11.1")

    runtimeOnly("ch.qos.logback:logback-classic:1.4.7")

    annotationProcessor("io.micronaut:micronaut-inject-java")
}

micronaut {
    version("4.3.4")
    runtime("netty")
    processing {
        incremental(true)
        annotations("com.ai2dev.akhil_randomnumber.*")
    }
}

application {
    mainClass.set("com.ai2dev.akhil_randomnumber.Application")
}

tasks.withType<JavaCompile> {
    options.encoding = "UTF-8"
}
