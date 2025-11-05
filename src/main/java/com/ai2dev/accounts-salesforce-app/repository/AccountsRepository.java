package com.ai2dev.accountssalesforceapp.repository;

import com.ai2dev.accountssalesforceapp.model.Account;
import jakarta.inject.Inject;
import jakarta.inject.Singleton;

import java.util.ArrayList;
import java.util.List;

@Singleton
public class AccountsRepository {

    @Inject
    protected SalesforceConnector connector;

    public List<Account> create(List<Account> accounts) {
        // Accept multiple accounts and create them one by one in Salesforce
        List<Account> created = new ArrayList<>();
        for (Account a : accounts) {
            Account c = connector.createAccount(a);
            created.add(c);
        }
        return created;
    }

    public List<Account> findAll() {
        return connector.queryAccounts();
    }

    public Account findById(String id) {
        return connector.getAccount(id);
    }

    public Account update(Account account) {
        return connector.updateAccount(account);
    }

    public boolean delete(String id) {
        return connector.deleteAccount(id);
    }
}
