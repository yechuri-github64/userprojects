package com.ai2dev.hellojava.service; import com.ai2dev.hellojava.model.Account; import com.ai2dev.hellojava.repository.AccountRepository; import jakarta.inject.Inject; import jakarta.inject.Singleton; import java.util.List; @Singleton
public class AccountService { @Inject
 AccountRepository accountRepository; public List<Account> getAllAccounts () { return accountRepository.findAll (); }

 public Account createAccount (Account account) { return accountRepository.save (account); }

 public Account updateAccount (String id, Account account) { return accountRepository.update (id, account); }

 public void deleteAccount (String id) { accountRepository.delete (id); }
}