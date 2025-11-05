# Micronaut Lambda Example

This project is a minimal Micronaut application packaged for AWS Lambda (Micronaut runtime: aws_lambda). It includes a simple controller and a Lambda handler that extends Micronaut's AWS proxy handler.

How to build:

- ./gradlew assemble

Deploy the resulting artifact to AWS Lambda and point the handler to example.micronaut.LambdaHandler::handleRequest (the Micronaut handler class extends the AWS RequestStreamHandler implementation provided by Micronaut).
