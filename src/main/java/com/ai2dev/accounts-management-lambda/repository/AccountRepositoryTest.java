package com.ai2dev.accountsmanagementlambda.repository; import com.ai2dev.accountsmanagementlambda.model.Account; import io.micronaut.test.extensions.junit5.annotation.MicronautTest; import jakarta.inject.Inject; import org.junit.jupiter.api.Test; import static org.junit.jupiter.api.Assertions. *; @MicronautTest
public class AccountRepositoryTest { @Inject
 AccountRepository accountRepository; @Test
 public void testSaveAccount () { Account account = new Account (null, "Charlie", "charlie@example.com", "789 Pine Rd"); accountRepository.save (account); assertNotNull (account.getId () ); }
}