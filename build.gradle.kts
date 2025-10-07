plugins {
    id("io.micronaut.application") version "4.3.4"
    id("io.micronaut.aot") version "4.3.4"
    id("java")
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

    annotationProcessor("io.micronaut:micronaut-inject-java")
    annotationProcessor("io.micronaut.data:micronaut-data-processor")

    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut:micronaut-http-server-netty")
    implementation("io.micronaut:micronaut-jackson-databind")
    implementation("io.micronaut.validation:micronaut-validation")

    implementation("io.micronaut.data:micronaut-data-hibernate-jpa")
    implementation("io.micronaut.sql:micronaut-jdbc-hikari")

    runtimeOnly("ch.qos.logback:logback-classic")
    runtimeOnly("com.mysql:mysql-connector-j:8.3.0")
}

application {
    mainClass.set("com.ai2dev.get_accounts.Application")
}
tasks.withType<Jar> {
    manifest {
        attributes(
            "Main-Class" to application.mainClass.get()
        )
    }
}

micronaut {
    version.set("4.3.4")
    runtime("netty")
    processing {
        incremental(true)
        annotations("com.ai2dev.get_accounts.*")
    }
    aot {
        optimizeServiceLoading.set(true)
        //convertYamlToJava.set(true)
        precomputeOperations.set(true)
        cacheEnvironment.set(true)
    }
}
