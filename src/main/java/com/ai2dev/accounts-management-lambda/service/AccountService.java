package com.ai2dev.accountsmanagementlambda.service;

import com.ai2dev.accountsmanagementlambda.model.Account;
import com.ai2dev.accountsmanagementlambda.repository.AccountRepository;
import javax.inject.Inject;
import javax.inject.Singleton;
import java.util.List;

@Singleton
public class AccountService {

    @Inject
    AccountRepository accountRepository;

    public List<Account> listAccounts() {
        return accountRepository.findAll();
    }

    public Account createAccount(Account account) {
        return accountRepository.save(account);
    }

    public Account updateAccount(Long id, Account account) {
        account.setId(id);
        return accountRepository.update(account);
    }

    public void deleteAccount(Long id) {
        accountRepository.deleteById(id);
    }
}