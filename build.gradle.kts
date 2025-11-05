plugins {
    id("io.micronaut.application") version "4.3.4"
    id("io.micronaut.aot") version "4.3.4"
    id("com.github.johnrengelman.shadow") version "8.1.1"
    java
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
    // BOM import for Micronaut artifacts
    implementation(platform("io.micronaut:micronaut-bom:3.9.7"))

    // Micronaut core
    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut:micronaut-http-server-netty")
    implementation("io.micronaut:micronaut-http-client")
    implementation("io.micronaut:micronaut-validation")
    implementation("io.micronaut:micronaut-jackson-databind")

    // YAML support
    implementation("org.yaml:snakeyaml:2.0")

    // Salesforce REST helper library (external connector)
    implementation("com.force.api:force-rest-api:0.0.9")

    // Logging
    implementation("ch.qos.logback:logback-classic:1.4.11")

    // Annotation processing (Micronaut inject)
    annotationProcessor("io.micronaut:micronaut-inject-java")
}

application {
    mainClass.set("com.ai2dev.accountssalesforceapp.Application")
}

micronaut {
    version("4.3.4")
    runtime("netty")
    aot {
        optimizeServiceLoading.set(true)
    }
    processing {
        incremental(true)
        annotations("com.ai2dev.accountssalesforceapp.*")
    }
}

// Shadow configuration for fat jar
tasks.named<com.github.jengelman.gradle.plugins.shadow.tasks.ShadowJar>("shadowJar") {
    mergeServiceFiles()
}

tasks {
    "run"(JavaExec::class.java) {
        classpath = sourceSets.main.get().runtimeClasspath
        mainClass.set("com.ai2dev.accountssalesforceapp.Application")
    }
}
