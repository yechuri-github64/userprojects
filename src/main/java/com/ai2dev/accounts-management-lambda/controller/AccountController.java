package com.ai2dev.accountsmanagementlambda.controller; import com.ai2dev.accountsmanagementlambda.model.Account; import com.ai2dev.accountsmanagementlambda.service.AccountService; import io.micronaut.http.annotation.Controller; import io.micronaut.http.annotation.Delete; import io.micronaut.http.annotation.Get; import io.micronaut.http.annotation.Post; import io.micronaut.http.annotation.Put; import io.micronaut.http.annotation.QueryValue; import io.micronaut.http.annotation.Body; import java.util.List; @Controller (" / accounts") public class AccountController { private final AccountService accountService; public AccountController (AccountService accountService) { this.accountService = accountService; }

 @Get
 public Account getAccount (@QueryValue ("id") Long id) { return accountService.getAccount (id); }

 @Post
 public List<Account> createAccounts (@Body List<Account> accounts) { return accountService.createAccounts (accounts); }

 @Put
 public Account updateAccount (@QueryValue ("id") Long id, @Body Account account) { return accountService.updateAccount (id, account); }

 @Delete
 public boolean deleteAccount (@QueryValue ("id") Long id) { return accountService.deleteAccount (id); }
}