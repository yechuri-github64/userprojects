package com.example;

import io.micronaut.function.aws.MicronautRequestHandler;
import java.util.Map;
import java.util.HashMap;

/**
 * AWS Lambda handler that delegates to Micronaut DI context.
 * It extends MicronautRequestHandler so Micronaut can inject beans into this handler.
 */
public class FunctionRequestHandler extends MicronautRequestHandler<Map<String, Object>, Map<String, Object>> {

    @Override
    public Map<String, Object> execute(Map<String, Object> input) {
        Map<String, Object> response = new HashMap<>();
        response.put("message", "Hello from Micronaut AWS Lambda!");
        response.put("received", input);
        return response;
    }
}
