package com.ai2dev.test_project_java.controller;

import io.micronaut.http.annotation.*;
import io.micronaut.http.HttpResponse;
import jakarta.inject.Inject;
import java.util.List;
import java.util.Optional;

import com.ai2dev.test_project_java.model.Account;
import com.ai2dev.test_project_java.dto.AccountDto;
import com.ai2dev.test_project_java.service.AccountService;

@Controller("/accounts")
public class AccountController {

    private final AccountService accountService;

    @Inject
    public AccountController(AccountService accountService) {
        this.accountService = accountService;
    }

    @Get("/")
    public List<Account> list() {
        return accountService.getAll();
    }

    @Get("/{id}")
    public HttpResponse<Account> get(Long id) {
        Optional<Account> acc = accountService.getById(id);
        return acc.map(HttpResponse::ok).orElseGet(() -> HttpResponse.notFound());
    }

    @Post("/")
    public HttpResponse<List<Account>> createMultiple(@Body List<AccountDto> dtos) {
        List<Account> created = accountService.createMultiple(dtos);
        return HttpResponse.created(created);
    }

    @Put("/{id}")
    public HttpResponse<Account> update(Long id, @Body AccountDto dto) {
        Account updated = accountService.update(id, dto);
        if (updated == null) return HttpResponse.notFound();
        return HttpResponse.ok(updated);
    }

    @Delete("/{id}")
    public HttpResponse<?> delete(Long id) {
        accountService.delete(id);
        return HttpResponse.noContent();
    }
}
