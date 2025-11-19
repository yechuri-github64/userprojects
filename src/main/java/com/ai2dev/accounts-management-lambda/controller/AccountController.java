package com.ai2dev.accountsmanagementlambda.controller; import com.ai2dev.accountsmanagementlambda.model.Account; import com.ai2dev.accountsmanagementlambda.service.AccountService; import io.micronaut.http.HttpResponse; import io.micronaut.http.annotation.Controller; import io.micronaut.http.annotation.Delete; import io.micronaut.http.annotation.Get; import io.micronaut.http.annotation.Post; import io.micronaut.http.annotation.Put; import io.micronaut.http.annotation.QueryValue; import jakarta.inject.Inject; import java.util.List; @Controller (" / accounts") public class AccountController { @Inject
 AccountService accountService; @Get
 public HttpResponse<Account> getAccount (@QueryValue Long id) { return HttpResponse.ok (accountService.getAccount (id) ); }

 @Post
 public HttpResponse<List<Account>> createAccounts (List<Account> accounts) { return HttpResponse.ok (accountService.createAccounts (accounts) ); }

 @Put
 public HttpResponse<Account> updateAccount (Long id, Account account) { return HttpResponse.ok (accountService.updateAccount (id, account) ); }

 @Delete
 public HttpResponse<Void> deleteAccount (@QueryValue Long id) { accountService.deleteAccount (id); return HttpResponse.noContent (); }
}