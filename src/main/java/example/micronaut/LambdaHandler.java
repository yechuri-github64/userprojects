package example.micronaut;

import io.micronaut.function.aws.proxy.MicronautLambdaHandler;

/**
 * AWS Lambda handler adapter. Extends Micronaut's provided Lambda handler which
 * wires up the Micronaut application context and routes API Gateway requests
 * to Micronaut controllers.
 */
public class LambdaHandler extends MicronautLambdaHandler {
    public LambdaHandler() {
        super();
    }
}
