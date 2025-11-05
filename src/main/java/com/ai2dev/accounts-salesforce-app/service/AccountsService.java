package com.ai2dev.accountssalesforceapp.service;

import com.ai2dev.accountssalesforceapp.model.Account;
import com.ai2dev.accountssalesforceapp.repository.AccountsRepository;
import jakarta.inject.Inject;
import jakarta.inject.Singleton;

import java.util.List;

@Singleton
public class AccountsService {

    @Inject
    protected AccountsRepository repository;

    public List<Account> createAccounts(List<Account> accounts) {
        return repository.create(accounts);
    }

    public List<Account> getAllAccounts() {
        return repository.findAll();
    }

    public Account getById(String id) {
        return repository.findById(id);
    }

    public Account updateAccount(String id, Account update) {
        update.setId(id);
        return repository.update(update);
    }

    public boolean deleteAccount(String id) {
        return repository.delete(id);
    }
}
