package com.ai2dev.newtempnum.controller;

import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Get;
import com.ai2dev.newtempnum.service.NumberService;
import javax.inject.Inject;
import java.util.List;

@Controller("/numbers")
public class NumberController {
  @Inject
  private NumberService numberService;

  @Get("/even")
  public List<Integer> getEvenNumbers() {
    return numberService.getEvenNumbers();
  }
}