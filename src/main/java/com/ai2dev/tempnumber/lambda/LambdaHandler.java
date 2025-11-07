package com.ai2dev.tempnumber.lambda;

import com.amazonaws.services.lambda.runtime.Context;
import com.amazonaws.services.lambda.runtime.RequestHandler;
import com.ai2dev.tempnumber.service.NumberService;

public class LambdaHandler implements RequestHandler<Object, Integer> {
  private final NumberService numberService;

  public LambdaHandler(NumberService numberService) {
    this.numberService = numberService;
  }

  @Override
  public Integer handleRequest(Object input, Context context) {
    return numberService.generateRandomNumber();
  }
}