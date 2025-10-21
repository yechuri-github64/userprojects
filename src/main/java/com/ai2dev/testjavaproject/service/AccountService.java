package com.ai2dev.testjavaproject.service;

import jakarta.inject.Singleton;
import java.util.Optional;

import com.ai2dev.testjavaproject.model.Account;
import com.ai2dev.testjavaproject.repository.AccountRepository;

@Singleton
public class AccountService {

    private final AccountRepository repository;

    public AccountService(AccountRepository repository) {
        this.repository = repository;
    }

    public Iterable<Account> list() {
        return repository.findAll();
    }

    public Optional<Account> get(Long id) {
        return repository.findById(id);
    }

    public Iterable<Account> createBulk(Iterable<Account> accounts) {
        return repository.saveAll(accounts);
    }

    public Account update(Long id, Account updated) {
        Optional<Account> existing = repository.findById(id);
        if (existing.isPresent()) {
            Account e = existing.get();
            e.setName(updated.getName());
            e.setEmail(updated.getEmail());
            e.setAddress(updated.getAddress());
            repository.update(e);
            return e;
        }
        throw new RuntimeException("Account not found: " + id);
    }

    public void delete(Long id) {
        repository.deleteById(id);
    }
}
