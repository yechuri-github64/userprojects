package com.ai2dev.accountsmanagementlambda.model; import org.junit.jupiter.api.Test; import static org.junit.jupiter.api.Assertions. *; public class AccountTest { @Test
 public void testAccountCreation () { Account account = new Account (null, "Charlie", "charlie@example.com", "789 Pine Rd"); assertEquals ("Charlie", account.getName () ); }
}