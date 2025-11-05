package com.ai2dev.randomlambda1;

import com.ai2dev.randomlambda1.service.RandomService;
import io.micronaut.test.extensions.junit5.annotation.MicronautTest;
import jakarta.inject.Inject;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertTrue;

@MicronautTest
public class RandomServiceTest {

    @Inject
    RandomService randomService;

    @Test
    void generatesAbove100() {
        int val = randomService.generateAbove(100).getValue();
        assertTrue(val > 100);
    }
}
