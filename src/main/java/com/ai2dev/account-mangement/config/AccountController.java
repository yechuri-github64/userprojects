package com.ai2dev.account-mangement.controller;

import com.ai2dev.account-mangement.model.Account;
import com.ai2dev.account-mangement.service.AccountService;
import io.micronaut.http.HttpResponse;
import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Delete;
import io.micronaut.http.annotation.Get;
import io.micronaut.http.annotation.Post;
import io.micronaut.http.annotation.Put;
import io.micronaut.http.annotation.Status;

import javax.inject.Inject;
import java.util.List;

@Controller("/accounts")
public class AccountController {

    @Inject
    private AccountService accountService;

    @Get
    public List<Account> list() {
        return accountService.listAccounts();
    }

    @Post
    @Status(201)
    public HttpResponse<Account> create(Account account) {
        return HttpResponse.created(accountService.createAccount(account));
    }

    @Put("/{id}")
    public HttpResponse<Account> update(Long id, Account account) {
        return HttpResponse.ok(accountService.updateAccount(id, account));
    }

    @Delete("/{id}")
    public HttpResponse<Void> delete(Long id) {
        accountService.deleteAccount(id);
        return HttpResponse.noContent();
    }
}
