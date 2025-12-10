plugins {
    id("io.micronaut.application") version "4.3.4"
    id("com.github.johnrengelman.shadow") version "8.1.1"
}

micronaut {
    version("4.3.4")
    runtime("netty")
}

dependencies {
    implementation(platform("io.micronaut:micronaut-bom:3.9.7"))
    implementation("io.micronaut:micronaut-http-client")
    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut.sql:micronaut-jdbc")
    implementation("mysql:mysql-connector-java:8.0.30")
    implementation("org.yaml:snakeyaml:1.30")
}

application {
    mainClass.set("com.ai2dev.account-mangement.Application")
}
