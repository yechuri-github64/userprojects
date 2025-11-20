package com.ai2dev.accountsmanagementlambda.controller;

import com.ai2dev.accountsmanagementlambda.model.Account;
import com.ai2dev.accountsmanagementlambda.service.AccountService;
import io.micronaut.http.HttpResponse;
import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Delete;
import io.micronaut.http.annotation.Get;
import io.micronaut.http.annotation.Post;
import io.micronaut.http.annotation.Put;
import io.micronaut.http.annotation.Body;

import javax.inject.Inject;
import java.util.List;

@Controller("/accounts")
public class AccountController {

    @Inject
    AccountService accountService;

    @Get
    public List<Account> list() {
        return accountService.listAccounts();
    }

    @Post
    public HttpResponse<Account> create(@Body Account account) {
        return HttpResponse.created(accountService.createAccount(account));
    }

    @Put("/{id}")
    public HttpResponse<Account> update(Long id, @Body Account account) {
        return HttpResponse.ok(accountService.updateAccount(id, account));
    }

    @Delete("/{id}")
    public HttpResponse<Void> delete(Long id) {
        accountService.deleteAccount(id);
        return HttpResponse.noContent();
    }
}