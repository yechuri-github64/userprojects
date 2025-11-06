package com.ai2dev.akhil_randomnumber.service;

import javax.inject.Singleton;
import java.util.concurrent.ThreadLocalRandom;

@Singleton
public class RandomNumberService {

    public int generateGreaterThan500AndLessThan1000() {
        // ThreadLocalRandom.nextInt(origin, bound) -> origin (inclusive), bound (exclusive)
        // To get numbers >500 and <1000 we use 501..999
        return ThreadLocalRandom.current().nextInt(501, 1000);
    }
}
