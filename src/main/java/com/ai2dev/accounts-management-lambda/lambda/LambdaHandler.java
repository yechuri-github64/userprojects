package com.ai2dev.accountsmanagementlambda.lambda; import com.amazonaws.services.lambda.runtime.Context; import com.amazonaws.services.lambda.runtime.RequestHandler; import com.ai2dev.accountsmanagementlambda.controller.AccountController; public class LambdaHandler implements RequestHandler<Object, String> { private final AccountController accountController; public LambdaHandler () { this.accountController = new AccountController (); }

 @Override
 public String handleRequest (Object input, Context context) { / / Handle the input and call the appropriate controller methods
 return "Handled"; }
}