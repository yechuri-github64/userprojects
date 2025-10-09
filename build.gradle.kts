import io.micronaut.gradle.MicronautRuntime
plugins {
    id("io.micronaut.application") version "4.3.4"
    id("io.micronaut.aot") version "4.3.4"
    id("com.github.johnrengelman.shadow") version "8.1.1"
   // id("com.gradleup.shadow") version "8.3.0"
    java
}

val micronautVersion = "4.3.4"

group = "com.ai2dev"
version = "0.1"

repositories {
    mavenCentral()
}

java {
    toolchain {
        languageVersion.set(JavaLanguageVersion.of(17))
    }
}

dependencies {
    // Import Micronaut BOM
    implementation(platform("io.micronaut:micronaut-bom:3.9.7"))

    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut:micronaut-http-client")
    implementation("io.micronaut:micronaut-http-server-netty")
    implementation("io.micronaut.data:micronaut-data-jdbc")
    implementation("io.micronaut:micronaut-jackson-databind")
    implementation("io.micronaut:micronaut-validation")

    // YAML support
    implementation("org.yaml:snakeyaml:2.0")

    // MySQL connector (explicit version as backend connector)
    runtimeOnly("mysql:mysql-connector-java:8.0.33")

    // Annotation processors
    annotationProcessor("io.micronaut.data:micronaut-data-processor")
    annotationProcessor("io.micronaut:micronaut-inject-java")
}

application {
    mainClass.set("com.ai2dev.testcontractsmgmt.Application")
}

micronaut {
    runtime.set(MicronautRuntime.NETTY)
    testRuntime.set(io.micronaut.gradle.MicronautRuntime.JUNIT)
    processing {
        incremental.set(true)
        annotations.add("com.ai2dev.testcontractsmgmt.*")
    }
}

tasks.withType<JavaCompile>().configureEach {
    options.encoding = "UTF-8"
    options.release.set(17)
}
