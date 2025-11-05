package com.ai2dev.accountssalesforceapp.repository;

import com.ai2dev.accountssalesforceapp.config.SalesforceConfig;
import com.ai2dev.accountssalesforceapp.model.Account;
import com.force.api.ForceApi;
import com.force.api.ForceApiConfig;
import com.force.api.QueryResult;
import jakarta.annotation.PostConstruct;
import jakarta.inject.Inject;
import jakarta.inject.Singleton;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@Singleton
public class SalesforceConnector {

    @Inject
    protected SalesforceConfig config;

    private ForceApi api;

    @PostConstruct
    public void init() {
        ForceApiConfig cfg = new ForceApiConfig()
                .setLoginEndpoint(config.getLoginUrl())
                .setUsername(config.getUsername())
                .setPassword(config.getPassword() + config.getSecurityToken())
                .setClientId(config.getClientId())
                .setClientSecret(config.getClientSecret());
        api = new ForceApi(cfg);
    }

    public Account createAccount(Account a) {
        Map<String, Object> record = new HashMap<>();
        record.put("Name", a.getName());
        if (a.getPhone() != null) record.put("Phone", a.getPhone());
        if (a.getWebsite() != null) record.put("Website", a.getWebsite());
        if (a.getIndustry() != null) record.put("Industry", a.getIndustry());

        Map<String, Object> result = api.createSObject("Account", record);
        String id = (String) result.get("id");
        a.setId(id);
        return a;
    }

    public List<Account> queryAccounts() {
        QueryResult<Map> qr = api.query("SELECT Id, Name, Phone, Website, Industry FROM Account");
        List<Account> list = new ArrayList<>();
        for (Map r : qr.getRecords()) {
            Account a = mapToAccount(r);
            list.add(a);
        }
        return list;
    }

    public Account getAccount(String id) {
        try {
            Map<String, Object> r = api.getSObject("Account", id);
            return mapToAccount(r);
        } catch (Exception e) {
            return null;
        }
    }

    public Account updateAccount(Account account) {
        Map<String, Object> record = new HashMap<>();
        if (account.getName() != null) record.put("Name", account.getName());
        if (account.getPhone() != null) record.put("Phone", account.getPhone());
        if (account.getWebsite() != null) record.put("Website", account.getWebsite());
        if (account.getIndustry() != null) record.put("Industry", account.getIndustry());
        api.updateSObject("Account", account.getId(), record);
        return account;
    }

    public boolean deleteAccount(String id) {
        try {
            api.deleteSObject("Account", id);
            return true;
        } catch (Exception e) {
            return false;
        }
    }

    @SuppressWarnings("unchecked")
    private Account mapToAccount(Map r) {
        Account a = new Account();
        Object id = r.get("Id");
        if (id == null) id = r.get("id");
        a.setId(id != null ? id.toString() : null);
        Object name = r.get("Name");
        a.setName(name != null ? name.toString() : null);
        Object phone = r.get("Phone");
        a.setPhone(phone != null ? phone.toString() : null);
        Object website = r.get("Website");
        a.setWebsite(website != null ? website.toString() : null);
        Object industry = r.get("Industry");
        a.setIndustry(industry != null ? industry.toString() : null);
        return a;
    }
}
