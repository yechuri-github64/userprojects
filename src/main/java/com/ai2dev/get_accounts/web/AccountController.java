package com.ai2dev.get_accounts.web;

import com.ai2dev.get_accounts.dto.AccountCreateRequest;
import com.ai2dev.get_accounts.dto.AccountResponse;
import com.ai2dev.get_accounts.dto.AccountUpdateRequest;
import com.ai2dev.get_accounts.service.AccountService;
import io.micronaut.http.HttpResponse;
import io.micronaut.http.MediaType;
import io.micronaut.http.annotation.*;
import io.micronaut.validation.Validated;
import jakarta.validation.Valid;
import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Positive;
import jakarta.validation.constraints.Size;

import java.util.List;

@Validated
@Controller("/api/accounts")
public class AccountController {

    private final AccountService accountService;

    public AccountController(AccountService accountService) {
        this.accountService = accountService;
    }

    @Get(produces = MediaType.APPLICATION_JSON)
    public HttpResponse<List<AccountResponse>> list(@QueryValue(value = "limit", defaultValue = "5") @Positive @Max(5) Integer limit) {
        return HttpResponse.ok(accountService.getAccounts(limit));
    }

    @Post(consumes = MediaType.APPLICATION_JSON, produces = MediaType.APPLICATION_JSON)
    public HttpResponse<List<AccountResponse>> create(@Body @Valid @Size(min = 1, max = 5) List<@Valid AccountCreateRequest> requests) {
        return HttpResponse.created(accountService.createAccounts(requests));
    }

    @Put(uri = "/{id}", consumes = MediaType.APPLICATION_JSON, produces = MediaType.APPLICATION_JSON)
    public HttpResponse<AccountResponse> update(@PathVariable Long id, @Body @Valid AccountUpdateRequest request) {
        return HttpResponse.ok(accountService.updateAccount(id, request));
    }

    @Delete(uri = "/{id}")
    public HttpResponse<?> delete(@PathVariable Long id) {
        accountService.deleteAccount(id);
        return HttpResponse.noContent();
    }
}
