package com.ai2dev.accountsmanagementlambda.lambda; import com.amazonaws.services.lambda.runtime.Context; import com.amazonaws.services.lambda.runtime.RequestHandler; import com.ai2dev.accountsmanagementlambda.controller.AccountController; import com.ai2dev.accountsmanagementlambda.model.Account; import java.util.List; public class LambdaHandler implements RequestHandler<List<Account>, String> { private final AccountController accountController; public LambdaHandler (AccountController accountController) { this.accountController = accountController; }

 @Override
 public String handleRequest (List<Account> input, Context context) { return accountController.createAccounts (input) .toString (); }
}