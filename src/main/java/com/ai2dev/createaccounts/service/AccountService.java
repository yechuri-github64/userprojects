package com.ai2dev.createaccounts.service;

import com.ai2dev.createaccounts.model.Account;
import com.ai2dev.createaccounts.repository.SalesforceConnector;
import jakarta.inject.Inject;
import jakarta.inject.Singleton;

@Singleton
public class AccountService {

    @Inject
    private SalesforceConnector salesforceConnector;

    public Account createAccount(Account account) {
        return salesforceConnector.createAccount(account);
    }
}
