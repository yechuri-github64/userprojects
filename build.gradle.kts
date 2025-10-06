plugins {
    id("io.micronaut.application") version "4.4.3"
    id("io.micronaut.aot") version "4.4.3"
    id("com.github.johnrengelman.shadow") version "8.1.1"
    java
}

repositories {
    mavenCentral()
}

java {
    sourceCompatibility = JavaVersion.VERSION_17
    targetCompatibility = JavaVersion.VERSION_17
}

micronaut {
    version.set("4.4.3")
    runtime("netty")
    processing {
        incremental(true)
        annotations("com.ai2dev.random_name.*")
    }
    aot {
        optimizeServiceLoading.set(true)
       // convertYamlToProperties.set(true)
    }
}

application {
    mainClass.set("com.ai2dev.random_name.Application")
}

dependencies {
    implementation(platform("io.micronaut.platform:micronaut-platform:4.3.8"))
    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut:micronaut-http-server-netty")
    implementation("io.micronaut:micronaut-http-client")
    implementation("io.micronaut:micronaut-inject")
    implementation("io.micronaut.validation:micronaut-validation")

    implementation("io.micronaut.serde:micronaut-serde-jackson")
    annotationProcessor("io.micronaut.serde:micronaut-serde-processor")

    implementation("io.micronaut.sql:micronaut-jdbc-hikari")
    implementation("io.micronaut.data:micronaut-data-jdbc")

    runtimeOnly("com.mysql:mysql-connector-j:8.3.0")
    runtimeOnly("com.microsoft.sqlserver:mssql-jdbc:12.6.1.jre11")
    runtimeOnly("com.oracle.database.jdbc:ojdbc11:23.3.0.23.09")

    runtimeOnly("ch.qos.logback:logback-classic:1.4.14")
}

tasks.withType<JavaCompile> {
    options.encoding = "UTF-8"
    options.release.set(17)
}
