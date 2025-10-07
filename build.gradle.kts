plugins {
    id("io.micronaut.application") version "4.3.5"
    id("io.micronaut.aot") version "4.3.5"
    id("java")
}

group = "com.ai2dev"
version = "0.1"

java {
    sourceCompatibility = JavaVersion.VERSION_17
    targetCompatibility = JavaVersion.VERSION_17
    toolchain {
        languageVersion.set(JavaLanguageVersion.of(17))
    }
}

repositories {
    mavenCentral()
}

dependencies {
    implementation(platform("io.micronaut:micronaut-bom:4.3.5"))
    annotationProcessor(platform("io.micronaut:micronaut-bom:4.3.5"))

    annotationProcessor("io.micronaut:micronaut-inject-java")
    annotationProcessor("io.micronaut.data:micronaut-data-processor")

    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut:micronaut-http-server-netty")
    implementation("io.micronaut:micronaut-jackson-databind")
    implementation("io.micronaut.validation:micronaut-validation")

    implementation("io.micronaut.data:micronaut-data-hibernate-jpa")
    implementation("io.micronaut.sql:micronaut-jdbc-hikari")
    implementation("io.micronaut.transaction:micronaut-transaction-annotations")

    runtimeOnly("ch.qos.logback:logback-classic")
    runtimeOnly("org.postgresql:postgresql")
}

application {
    mainClass.set("com.ai2dev.guess_name.Application")
}

micronaut {
    runtime("netty")
    processing {
        incremental(true)
        annotations("com.ai2dev.*")
    }
    aot {
        optimizeServiceLoading.set(true)
        convertYamlToProperties.set(true)
        precomputeOperations.set(true)
        cacheEnvironment.set(true)
        optimizeClassLoading.set(true)
    }
}
