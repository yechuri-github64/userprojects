package com.ai2dev.newtempnum.lambda;

import com.amazonaws.services.lambda.runtime.Context;
import com.amazonaws.services.lambda.runtime.RequestHandler;
import com.ai2dev.newtempnum.controller.NumberController;
import javax.inject.Inject;
import java.util.List;

public class LambdaHandler implements RequestHandler<Object, List<Integer>> {
  @Inject
  private NumberController numberController;

  @Override
  public List<Integer> handleRequest(Object input, Context context) {
    return numberController.getEvenNumbers();
  }
}