package com.ai2dev.accountssalesforceapp.controller;

import com.ai2dev.accountssalesforceapp.model.Account;
import com.ai2dev.accountssalesforceapp.service.AccountsService;
import io.micronaut.http.HttpResponse;
import io.micronaut.http.annotation.*;

import jakarta.inject.Inject;
import java.util.List;

@Controller("/accounts")
public class AccountsController {

    @Inject
    protected AccountsService accountsService;

    @Post("/bulk")
    public HttpResponse<List<Account>> createBulk(@Body List<Account> accounts) {
        List<Account> created = accountsService.createAccounts(accounts);
        return HttpResponse.created(created);
    }

    @Get
    public HttpResponse<List<Account>> list() {
        List<Account> accounts = accountsService.getAllAccounts();
        return HttpResponse.ok(accounts);
    }

    @Get("/{id}")
    public HttpResponse<Account> getById(@PathVariable String id) {
        Account account = accountsService.getById(id);
        if (account == null) {
            return HttpResponse.notFound();
        }
        return HttpResponse.ok(account);
    }

    @Put("/{id}")
    public HttpResponse<Account> update(@PathVariable String id, @Body Account update) {
        Account updated = accountsService.updateAccount(id, update);
        if (updated == null) {
            return HttpResponse.notFound();
        }
        return HttpResponse.ok(updated);
    }

    @Delete("/{id}")
    public HttpResponse<Void> delete(@PathVariable String id) {
        boolean removed = accountsService.deleteAccount(id);
        if (!removed) {
            return HttpResponse.notFound();
        }
        return HttpResponse.noContent();
    }
}
