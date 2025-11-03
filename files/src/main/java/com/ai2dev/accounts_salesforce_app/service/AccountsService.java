package com.ai2dev.accounts_salesforce_app.service;

import com.ai2dev.accounts_salesforce_app.client.SalesforceClient;
import com.ai2dev.accounts_salesforce_app.dto.AccountRequest;
import com.ai2dev.accounts_salesforce_app.dto.AccountResponse;
import com.ai2dev.accounts_salesforce_app.model.Account;
import jakarta.inject.Singleton;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@Singleton
public class AccountsService {
    private final SalesforceClient salesforceClient;

    public AccountsService(SalesforceClient salesforceClient) {
        this.salesforceClient = salesforceClient;
    }

    public List<AccountResponse> createAccounts(List<AccountRequest> requests) {
        List<Account> accounts = requests.stream().map(r -> {
            Account a = new Account();
            a.setName(r.getName());
            a.setPhone(r.getPhone());
            a.setWebsite(r.getWebsite());
            return a;
        }).collect(Collectors.toList());
        List<Map<String, Object>> created = salesforceClient.createAccounts(accounts);
        return created.stream().map(m -> {
            AccountResponse ar = new AccountResponse();
            ar.setRaw(m);
            Object id = m.get("id");
            if (id == null) id = m.get("Id");
            if (id != null) ar.setId(id.toString());
            return ar;
        }).collect(Collectors.toList());
    }

    public AccountResponse getAccount(String id) {
        Map m = salesforceClient.getAccount(id);
        if (m == null) return null;
        AccountResponse ar = new AccountResponse();
        ar.setId(id);
        ar.setRaw(m);
        return ar;
    }

    public AccountResponse updateAccount(String id, AccountRequest req) {
        Map<String, Object> updates = new HashMap<>();
        if (req.getName() != null) updates.put("Name", req.getName());
        if (req.getPhone() != null) updates.put("Phone", req.getPhone());
        if (req.getWebsite() != null) updates.put("Website", req.getWebsite());
        Map res = salesforceClient.updateAccount(id, updates);
        AccountResponse ar = new AccountResponse();
        ar.setId(id);
        ar.setRaw(res);
        return ar;
    }

    public Map<String, Object> deleteAccount(String id) {
        boolean ok = salesforceClient.deleteAccount(id);
        Map<String, Object> res = new HashMap<>();
        res.put("id", id);
        res.put("deleted", ok);
        return res;
    }
}
