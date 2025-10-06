import io.micronaut.gradle.MicronautRuntime

plugins {
    id("java")
    id("io.micronaut.application") version "4.8.0"
}

group = "com.ai2dev"
version = "0.1"

java {
    sourceCompatibility = JavaVersion.toVersion("21")
    targetCompatibility = JavaVersion.toVersion("21")
}

micronaut {
    runtime(MicronautRuntime.NETTY)
    testRuntime("junit5")
    processing {
        incremental(true)
        annotations("com.ai2dev.random_name.*")
    }
}

repositories {
    mavenCentral()
}

dependencies {
    implementation(platform("io.micronaut:micronaut-bom:4.6.3"))

    implementation("io.micronaut:micronaut-http-server-netty")
    implementation("io.micronaut:micronaut-runtime")
    implementation("io.micronaut:micronaut-http-client")
    implementation("io.micronaut:micronaut-inject")
    implementation("io.micronaut:micronaut-validation")

    // Micronaut Data JDBC
    implementation("io.micronaut.data:micronaut-data-jdbc")
    implementation("io.micronaut.sql:micronaut-jdbc-hikari")

    // Database drivers (runtime)
    runtimeOnly("mysql:mysql-connector-java")
    runtimeOnly("com.microsoft.sqlserver:mssql-jdbc")
    // Oracle driver (note: may require manual setup for some versions/repos)
    runtimeOnly("com.oracle.database.jdbc:ojdbc10:21.11.0.0")

    // JSON
    implementation("com.fasterxml.jackson.core:jackson-databind")

    // HTTP/External integrations
    implementation("io.micronaut:micronaut-http-client")
    implementation("org.apache.httpcomponents.client5:httpclient5:5.2.1")

    annotationProcessor("io.micronaut:micronaut-inject-java")
    annotationProcessor("io.micronaut.data:micronaut-data-processor")

    testImplementation("io.micronaut.test:micronaut-test-junit5")
    testImplementation("org.junit.jupiter:junit-jupiter-api")
}

application {
    mainClass.set("com.ai2dev.random_name.Application")
}

tasks.withType<Test> {
    useJUnitPlatform()
}
