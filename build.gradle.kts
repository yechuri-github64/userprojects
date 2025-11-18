plugins {id ("io.micronaut.application")version "4.6.1"
 id ("com.gradleup.shadow")version "8.3.9"
}

micronaut {runtime ("lambda_java")testRuntime ("junit5")processing {incremental (true)annotations ("com.ai2dev.hellojava.*")}
}

dependencies {implementation ("io.micronaut.platform:micronaut-platform:4.6.1")implementation (platform ("io.micronaut.aws:micronaut-aws-bom:4.6.1"))implementation ("io.micronaut.http-client")implementation ("io.micronaut.jackson")implementation ("jakarta.inject:jakarta.inject-api")testImplementation ("io.micronaut.test:micronaut-test-junit5")}

application {mainClass.set ("com.ai2dev.hellojava.Application")}