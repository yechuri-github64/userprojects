package com.ai2dev.accountsmanagementlambda.service; import com.ai2dev.accountsmanagementlambda.model.Account; import com.ai2dev.accountsmanagementlambda.repository.AccountRepository; import jakarta.inject.Inject; import jakarta.inject.Singleton; import java.util.List; @Singleton
public class AccountService { @Inject
 AccountRepository accountRepository; public Account getAccount (Long id) { return accountRepository.findById (id) .orElse (null); }

 public List<Account> createAccounts (List<Account> accounts) { return accountRepository.saveAll (accounts); }

 public Account updateAccount (Long id, Account account) { account.setId (id); return accountRepository.update (account); }

 public void deleteAccount (Long id) { accountRepository.deleteById (id); }
}