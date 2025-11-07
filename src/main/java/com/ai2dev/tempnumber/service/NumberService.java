package com.ai2dev.tempnumber.service;

import jakarta.inject.Singleton;
import java.util.Random;

@Singleton
public class NumberService {
  private final Random random = new Random();

  public int generateRandomNumber() {
    return 500 + random.nextInt(101);
  }
}