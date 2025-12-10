package com.ai2dev.account-mangement.service;

import com.ai2dev.account-mangement.model.Account;
import com.ai2dev.account-mangement.repository.AccountRepository;
import javax.inject.Inject;
import javax.inject.Singleton;
import java.util.List;

@Singleton
public class AccountService {

    @Inject
    private AccountRepository accountRepository;

    public List<Account> listAccounts() {
        return accountRepository.findAll();
    }

    public Account createAccount(Account account) {
        return accountRepository.save(account);
    }

    public Account updateAccount(Long id, Account account) {
        return accountRepository.update(id, account);
    }

    public void deleteAccount(Long id) {
        accountRepository.delete(id);
    }
}
