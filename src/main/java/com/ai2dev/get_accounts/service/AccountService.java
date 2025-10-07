package com.ai2dev.get_accounts.service;

import com.ai2dev.get_accounts.domain.Account;
import com.ai2dev.get_accounts.dto.AccountCreateRequest;
import com.ai2dev.get_accounts.dto.AccountResponse;
import com.ai2dev.get_accounts.dto.AccountUpdateRequest;
import com.ai2dev.get_accounts.mapper.AccountMapper;
import com.ai2dev.get_accounts.repository.AccountRepository;
import io.micronaut.data.model.Page;
import io.micronaut.data.model.Pageable;
import jakarta.inject.Singleton;
import jakarta.transaction.Transactional;

import java.util.List;
import java.util.NoSuchElementException;
import java.util.Objects;
import java.util.stream.Collectors;

@Singleton
public class AccountService {

    private static final int MAX_BULK = 5;
    private final AccountRepository repository;

    public AccountService(AccountRepository repository) {
        this.repository = repository;
    }

    @Transactional
    public List<AccountResponse> getAccounts(Integer limit) {
        int effectiveLimit = (limit == null || limit <= 0) ? MAX_BULK : Math.min(limit, MAX_BULK);
        Page<Account> page = repository.findAll(Pageable.from(0, effectiveLimit));
        return page.getContent().stream().map(AccountMapper::toResponse).collect(Collectors.toList());
    }

    @Transactional
    public List<AccountResponse> createAccounts(List<AccountCreateRequest> requests) {
        if (requests == null || requests.isEmpty()) {
            throw new IllegalArgumentException("At least 1 account must be provided to create.");
        }
        if (requests.size() > MAX_BULK) {
            throw new IllegalArgumentException("Cannot create more than " + MAX_BULK + " accounts per request.");
        }
        List<Account> entities = requests.stream().filter(Objects::nonNull).map(AccountMapper::toEntity).collect(Collectors.toList());
        List<Account> saved = repository.saveAll(entities);
        return saved.stream().map(AccountMapper::toResponse).collect(Collectors.toList());
    }

    @Transactional
    public AccountResponse updateAccount(Long id, AccountUpdateRequest request) {
        if (id == null) {
            throw new IllegalArgumentException("Account id is required for update.");
        }
        Account existing = repository.findById(id).orElseThrow(() -> new NoSuchElementException("Account not found with id=" + id));
        AccountMapper.applyUpdate(existing, request);
        Account updated = repository.update(existing);
        return AccountMapper.toResponse(updated);
    }

    @Transactional
    public void deleteAccount(Long id) {
        if (id == null) {
            throw new IllegalArgumentException("Account id is required for delete.");
        }
        Account existing = repository.findById(id).orElseThrow(() -> new NoSuchElementException("Account not found with id=" + id));
        repository.delete(existing);
    }
}
