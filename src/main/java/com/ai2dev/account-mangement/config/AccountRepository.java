package com.ai2dev.account-mangement.repository;

import com.ai2dev.account-mangement.model.Account;
import javax.inject.Singleton;
import java.util.ArrayList;
import java.util.List;

@Singleton
public class AccountRepository {

    private List<Account> accounts = new ArrayList<>();

    public List<Account> findAll() {
        return accounts;
    }

    public Account save(Account account) {
        accounts.add(account);
        return account;
    }

    public Account update(Long id, Account account) {
        // Update logic here
        return account;
    }

    public void delete(Long id) {
        // Delete logic here
    }
}
