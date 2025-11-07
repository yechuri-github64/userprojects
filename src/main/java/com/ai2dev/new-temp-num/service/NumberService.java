package com.ai2dev.newtempnum.service;

import javax.inject.Singleton;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

@Singleton
public class NumberService {
  public List<Integer> getEvenNumbers() {
    return IntStream.rangeClosed(100, 200)
      .filter(n -> n % 2 == 0)
      .boxed()
      .collect(Collectors.toList());
  }
}