package com.ai2dev.accountsmanagementlambda.service; import com.ai2dev.accountsmanagementlambda.model.Account; import com.ai2dev.accountsmanagementlambda.repository.AccountRepository; import jakarta.inject.Singleton; import java.util.List; @Singleton
public class AccountService { private final AccountRepository accountRepository; public AccountService (AccountRepository accountRepository) { this.accountRepository = accountRepository; }

 public Account getAccount (Long id) { return accountRepository.findById (id) .orElse (null); }

 public List<Account> createAccounts (List<Account> accounts) { return accountRepository.saveAll (accounts); }

 public Account updateAccount (Long id, Account account) { account.setId (id); return accountRepository.update (account); }

 public boolean deleteAccount (Long id) { return accountRepository.deleteById (id); }
}