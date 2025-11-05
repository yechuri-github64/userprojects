package com.example;

import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.*;
import java.util.Collections;
import java.util.Map;

public class FunctionRequestHandlerTest {

    @Test
    void testExecuteReturnsExpectedMap() {
        FunctionRequestHandler handler = new FunctionRequestHandler();
        Map<String, Object> result = handler.execute(Collections.singletonMap("key", "value"));
        assertNotNull(result);
        assertEquals("Hello from Micronaut AWS Lambda!", result.get("message"));
        assertTrue(result.containsKey("received"));
    }
}
