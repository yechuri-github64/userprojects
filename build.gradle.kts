plugins {
    java
    id("io.micronaut.application") version "4.3.4"
    id("io.micronaut.aot") version "4.3.4"
    id("com.github.johnrengelman.shadow") version "8.1.1"
}

val micronautVersion: String = "4.3.4"
val micronautBom: String = "3.9.7"
val snakeyamlVersion: String = "2.0"

group = "com.ai2dev.createaccounts"
version = "0.1"

repositories {
    mavenCentral()
}

dependencies {
    // Import Micronaut BOM as requested
    implementation(platform("io.micronaut:micronaut-bom:${micronautBom}"))

    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut:micronaut-http-server-netty")
    implementation("io.micronaut:micronaut-http-client")
    implementation("io.micronaut:micronaut-inject-java")
    implementation("io.micronaut:micronaut-validation")
    implementation("io.micronaut:micronaut-jackson-databind")

    // YAML processing
    implementation("org.yaml:snakeyaml:${snakeyamlVersion}")

    // JSON processing (explicit if required)
    implementation("com.fasterxml.jackson.core:jackson-databind:2.15.2")

    // Annotation processor for Micronaut (using BOM-managed versions)
    annotationProcessor("io.micronaut:micronaut-inject-java")
}

micronaut {
    version("4.3.4")
    runtime("netty")
    aot {
    isEnabled = false
    }
    processing {
        incremental(true)
        annotations("com.ai2dev.createaccounts.*")
    }
}

java {
    toolchain {
        languageVersion.set(JavaLanguageVersion.of(17))
    }
}

application {
    // main class for the application
    mainClass.set("com.ai2dev.createaccounts.Application")
}

tasks.named<Jar>("jar") {
    enabled = true
}
