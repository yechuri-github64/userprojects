package com.ai2dev.hellojava.controller; import com.ai2dev.hellojava.model.Account; import com.ai2dev.hellojava.service.AccountService; import io.micronaut.http.HttpResponse; import io.micronaut.http.annotation.Controller; import io.micronaut.http.annotation.Delete; import io.micronaut.http.annotation.Get; import io.micronaut.http.annotation.Post; import io.micronaut.http.annotation.Put; import jakarta.inject.Inject; import java.util.List; @Controller (" / accounts") public class AccountController { @Inject
 AccountService accountService; @Get
 public List<Account> getAllAccounts () { return accountService.getAllAccounts (); }

 @Post
 public HttpResponse<Account> createAccount (Account account) { return HttpResponse.created (accountService.createAccount (account) ); }

 @Put (" / { id}") public HttpResponse<Account> updateAccount (String id, Account account) { return HttpResponse.ok (accountService.updateAccount (id, account) ); }

 @Delete (" / { id}") public HttpResponse<Void> deleteAccount (String id) { accountService.deleteAccount (id); return HttpResponse.noContent (); }
}