package com.ai2dev.createaccounts.controller;

import com.ai2dev.createaccounts.model.Account;
import com.ai2dev.createaccounts.service.AccountService;
import io.micronaut.http.HttpResponse;
import io.micronaut.http.annotation.Body;
import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Post;
import jakarta.inject.Inject;

@Controller("/accounts")
public class AccountController {

    @Inject
    private AccountService accountService;

    @Post
    public HttpResponse<Account> createAccount(@Body Account account) {
        Account created = accountService.createAccount(account);
        return HttpResponse.created(created);
    }
}
