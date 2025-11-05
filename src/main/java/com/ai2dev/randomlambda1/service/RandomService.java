package com.ai2dev.randomlambda1.service;

import com.ai2dev.randomlambda1.model.RandomResponse;
import jakarta.inject.Singleton;
import java.time.Instant;
import java.util.concurrent.ThreadLocalRandom;

@Singleton
public class RandomService {

    public RandomResponse generateAbove(int minExclusive) {
        int base = Math.max(minExclusive, 100) + 1;
        int value = ThreadLocalRandom.current().nextInt(base, Integer.MAX_VALUE);
        return new RandomResponse(value, Instant.now().toString());
    }
}
