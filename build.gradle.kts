import org.gradle.jvm.tasks.Javadoc

val projectGroup = "com.ai2dev.testjavaproject"
val projectVersion = "0.1"
val javaVersion = 17
val micronautVersion = "4.3.4"
val micronautBomVersion = "3.9.7"
val aotVersion = "4.3.4"
val shadowVersion = "8.1.1"
val mysqlVersion = "8.1.0"
val snakeyamlVersion = "2.0"

plugins {
    id("java")
    id("application")
    id("com.github.johnrengelman.shadow") version shadowVersion
    id("io.micronaut.application") version micronautVersion
    id("io.micronaut.aot") version aotVersion
}

group = projectGroup
version = projectVersion

java {
    toolchain {
        languageVersion.set(JavaLanguageVersion.of(javaVersion))
    }
}

repositories {
    mavenCentral()
}

micronaut {
    version(micronautVersion)
    runtime("netty")
    processing {
        incremental(true)
        annotations("com.ai2dev.testjavaproject.*")
    }
}

dependencies {
    implementation(platform("io.micronaut:micronaut-bom:${micronautBomVersion}"))

    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut:micronaut-http-client")
    implementation("io.micronaut.data:micronaut-data-jdbc")
    implementation("io.micronaut.sql:micronaut-jdbc-hikari")
    implementation("io.micronaut:micronaut-http-server-netty")

    // YAML support
    implementation("org.yaml:snakeyaml:${snakeyamlVersion}")

    // MySQL connector with explicit version
    runtimeOnly("mysql:mysql-connector-java:${mysqlVersion}")

    annotationProcessor("io.micronaut:micronaut-inject-java")
}

application {
    mainClass.set("com.ai2dev.testjavaproject.Application")
}

tasks.withType<Javadoc> {
    (options as StandardJavadocDocletOptions).addStringOption("Xdoclint:none", "")
}
