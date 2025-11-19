package com.ai2dev.accountsmanagementlambda.lambda;
import com.amazonaws.services.lambda.runtime.Context;
import com.amazonaws.services.lambda.runtime.RequestHandler;
import com.ai2dev.accountsmanagementlambda.controller.AccountController;
import com.ai2dev.accountsmanagementlambda.model.Account;
import java.util.List;
public class LambdaHandler implements RequestHandler<Object, List<Account>> {
private final AccountController accountController;
public LambdaHandler()
{
this.accountController = new AccountController()
;
}
@Override public List<Account> handleRequest(Object input, Context context)
{
// Handle Lambda request return null;
}

}
