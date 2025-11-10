package com.ai2dev.akhilrandomnumber.service;

import javax.inject.Singleton;
import java.util.Random;

@Singleton
public class RandomNumberService {
    private final Random random = new Random();

    public int generateRandomNumber() {
        return random.nextInt(499) + 501;
    }
}