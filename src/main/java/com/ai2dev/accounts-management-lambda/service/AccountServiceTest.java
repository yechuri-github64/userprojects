package com.ai2dev.accountsmanagementlambda.service; import com.ai2dev.accountsmanagementlambda.model.Account; import com.ai2dev.accountsmanagementlambda.repository.AccountRepository; import io.micronaut.test.extensions.junit5.annotation.MicronautTest; import jakarta.inject.Inject; import org.junit.jupiter.api.Test; import java.util.List; import static org.junit.jupiter.api.Assertions. *; @MicronautTest
public class AccountServiceTest { @Inject
 AccountService accountService; @Inject
 AccountRepository accountRepository; @Test
 public void testCreateAccounts () { List<Account> accounts = List.of (new Account (null, "Charlie", "charlie@example.com", "789 Pine Rd"), new Account (null, "Dana", "dana@example.com", "101 Maple Blvd") ); List<Account> createdAccounts = accountService.createAccounts (accounts); assertEquals (2, createdAccounts.size () ); }
}