package com.ai2dev.testjavaproject.controller;

import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Get;
import io.micronaut.http.annotation.Post;
import io.micronaut.http.annotation.Put;
import io.micronaut.http.annotation.Delete;
import io.micronaut.http.annotation.Body;
import io.micronaut.http.HttpResponse;

import java.util.List;
import java.util.Optional;

import com.ai2dev.testjavaproject.model.Account;
import com.ai2dev.testjavaproject.service.AccountService;

@Controller("/accounts")
public class AccountController {

    private final AccountService service;

    public AccountController(AccountService service) {
        this.service = service;
    }

    @Get
    public Iterable<Account> list() {
        return service.list();
    }

    @Get("/{id}")
    public HttpResponse<Account> get(Long id) {
        Optional<Account> a = service.get(id);
        return a.map(HttpResponse::ok).orElse(HttpResponse.notFound());
    }

    @Post("/bulk")
    public HttpResponse<Iterable<Account>> createBulk(@Body List<Account> accounts) {
        Iterable<Account> created = service.createBulk(accounts);
        return HttpResponse.created(created);
    }

    @Put("/{id}")
    public HttpResponse<Account> update(Long id, @Body Account account) {
        try {
            Account updated = service.update(id, account);
            return HttpResponse.ok(updated);
        } catch (RuntimeException ex) {
            return HttpResponse.notFound();
        }
    }

    @Delete("/{id}")
    public HttpResponse<?> delete(Long id) {
        service.delete(id);
        return HttpResponse.noContent();
    }
}
