plugins {
    java
    id("io.micronaut.application") version "4.3.4"
    id("io.micronaut.aot") version "4.3.4"
    id("com.github.johnrengelman.shadow") version "8.1.1"
}

group = "com.ai2dev"
version = "0.1"

java {
    toolchain {
        languageVersion.set(JavaLanguageVersion.of(17))
    }
}

repositories {
    mavenCentral()
}

dependencies {
    implementation(platform("io.micronaut:micronaut-bom:3.9.7"))

    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut:micronaut-http-client")
    implementation("io.micronaut:micronaut-http-server-netty")
    implementation("io.micronaut:micronaut-jackson-databind")
    implementation("io.micronaut:micronaut-management")

    // SalesForce connector using HTTP client (Apache HttpClient 5) as backend connector
    implementation("org.apache.httpcomponents.client5:httpclient5:5.2.1")

    // YAML
    implementation("org.yaml:snakeyaml:2.1")

    // Annotation processing for Micronaut
    annotationProcessor("io.micronaut:micronaut-inject-java")
}

application {
    mainClass.set("com.ai2dev.accounts_salesforce_app.Application")
}

micronaut {
    version("4.3.4")
    runtime("netty")
    processing {
        incremental(true)
        annotations("com.ai2dev.accounts_salesforce_app.*")
    }
}
