package com.ai2dev.accountsmanagementlambda.lambda;

import com.amazonaws.services.lambda.runtime.Context;
import com.amazonaws.services.lambda.runtime.RequestHandler;
import com.ai2dev.accountsmanagementlambda.controller.AccountController;
import com.ai2dev.accountsmanagementlambda.model.Account;

public class LambdaHandler implements RequestHandler<Account, String> {

    private final AccountController accountController;

    public LambdaHandler() {
        this.accountController = new AccountController();
    }

    @Override
    public String handleRequest(Account input, Context context) {
        // Handle the request
        return "Account processed";
    }
}