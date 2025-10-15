package com.ai2dev.test_project_java.service.impl;

import com.ai2dev.test_project_java.service.AccountService;
import com.ai2dev.test_project_java.repository.AccountRepository;
import com.ai2dev.test_project_java.model.Account;
import com.ai2dev.test_project_java.dto.AccountDto;

import jakarta.inject.Inject;
import jakarta.inject.Singleton;
import jakarta.transaction.Transactional;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@Singleton
public class AccountServiceImpl implements AccountService {

    private final AccountRepository repository;

    @Inject
    public AccountServiceImpl(AccountRepository repository) {
        this.repository = repository;
    }

    @Override
    public List<Account> getAll() {
        List<Account> list = new ArrayList<>();
        repository.findAll().forEach(list::add);
        return list;
    }

    @Override
    public Optional<Account> getById(Long id) {
        return repository.findById(id);
    }

    @Override
    @Transactional
    public List<Account> createMultiple(List<AccountDto> dtos) {
        List<Account> saved = new ArrayList<>();
        for (AccountDto dto : dtos) {
            Account a = new Account(null, dto.getName(), dto.getEmail(), dto.getAddress());
            saved.add(repository.save(a));
        }
        return saved;
    }

    @Override
    @Transactional
    public Account update(Long id, AccountDto dto) {
        Optional<Account> opt = repository.findById(id);
        if (!opt.isPresent()) {
            return null;
        }
        Account a = opt.get();
        if (dto.getName() != null) a.setName(dto.getName());
        if (dto.getEmail() != null) a.setEmail(dto.getEmail());
        if (dto.getAddress() != null) a.setAddress(dto.getAddress());
        // repository.update may be available; fallback to save
        try {
            return repository.update(a);
        } catch (NoSuchMethodError | AbstractMethodError e) {
            return repository.save(a);
        }
    }

    @Override
    @Transactional
    public void delete(Long id) {
        repository.deleteById(id);
    }
}
