package com.ai2dev.newtempnum;

import io.micronaut.test.extensions.junit5.annotation.MicronautTest;
import org.junit.jupiter.api.Test;
import javax.inject.Inject;
import static org.junit.jupiter.api.Assertions.assertEquals;
import java.util.List;

@MicronautTest
public class NumberControllerTest {
  @Inject
  private NumberController numberController;

  @Test
  void testGetEvenNumbers() {
    List<Integer> evenNumbers = numberController.getEvenNumbers();
    assertEquals(51, evenNumbers.size());
  }
}