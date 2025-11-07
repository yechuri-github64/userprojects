package com.ai2dev.tempnumber.controller;

import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Get;
import com.ai2dev.tempnumber.service.NumberService;

@Controller("/number")
public class NumberController {
  private final NumberService numberService;

  public NumberController(NumberService numberService) {
    this.numberService = numberService;
  }

  @Get
  public int getNumber() {
    return numberService.generateRandomNumber();
  }
}