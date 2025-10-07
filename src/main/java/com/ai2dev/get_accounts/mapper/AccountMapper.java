package com.ai2dev.get_accounts.mapper;

import com.ai2dev.get_accounts.domain.Account;
import com.ai2dev.get_accounts.dto.AccountCreateRequest;
import com.ai2dev.get_accounts.dto.AccountResponse;
import com.ai2dev.get_accounts.dto.AccountUpdateRequest;

public final class AccountMapper {

    private AccountMapper() {}

    public static Account toEntity(AccountCreateRequest req) {
        Account a = new Account();
        a.setName(req.getName());
        a.setEmail(req.getEmail());
        a.setBalance(req.getBalance());
        a.setStatus(req.getStatus());
        return a;
        }

    public static void applyUpdate(Account entity, AccountUpdateRequest req) {
        if (req.getName() != null && !req.getName().isBlank()) {
            entity.setName(req.getName());
        }
        if (req.getEmail() != null && !req.getEmail().isBlank()) {
            entity.setEmail(req.getEmail());
        }
        if (req.getBalance() != null) {
            entity.setBalance(req.getBalance());
        }
        if (req.getStatus() != null) {
            entity.setStatus(req.getStatus());
        }
    }

    public static AccountResponse toResponse(Account entity) {
        AccountResponse r = new AccountResponse();
        r.setId(entity.getId());
        r.setName(entity.getName());
        r.setEmail(entity.getEmail());
        r.setBalance(entity.getBalance());
        r.setStatus(entity.getStatus());
        r.setCreatedAt(entity.getCreatedAt());
        r.setUpdatedAt(entity.getUpdatedAt());
        return r;
    }
}
