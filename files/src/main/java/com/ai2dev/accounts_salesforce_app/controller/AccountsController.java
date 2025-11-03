package com.ai2dev.accounts_salesforce_app.controller;

import com.ai2dev.accounts_salesforce_app.dto.AccountRequest;
import com.ai2dev.accounts_salesforce_app.dto.AccountResponse;
import com.ai2dev.accounts_salesforce_app.service.AccountsService;
import io.micronaut.http.HttpResponse;
import io.micronaut.http.annotation.*;
import jakarta.inject.Inject;

import java.util.List;
import java.util.Map;

@Controller("/api/accounts")
public class AccountsController {
    private final AccountsService service;

    @Inject
    public AccountsController(AccountsService service) {
        this.service = service;
    }

    @Post("/")
    public HttpResponse<List<AccountResponse>> create(@Body List<AccountRequest> requests) {
        List<AccountResponse> created = service.createAccounts(requests);
        return HttpResponse.created(created);
    }

    @Get("/{id}")
    public HttpResponse<AccountResponse> getById(@PathVariable String id) {
        AccountResponse ar = service.getAccount(id);
        if (ar == null) return HttpResponse.notFound();
        return HttpResponse.ok(ar);
    }

    @Put("/{id}")
    public HttpResponse<AccountResponse> update(@PathVariable String id, @Body AccountRequest request) {
        AccountResponse updated = service.updateAccount(id, request);
        return HttpResponse.ok(updated);
    }

    @Delete("/{id}")
    public HttpResponse<Map<String, Object>> delete(@PathVariable String id) {
        Map<String, Object> res = service.deleteAccount(id);
        return HttpResponse.ok(res);
    }
}
