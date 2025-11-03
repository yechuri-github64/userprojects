package com.ai2dev.accounts_salesforce_app.dto;

import java.util.Map;

public class AccountResponse {
    private String id;
    private Map raw;

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public Map getRaw() {
        return raw;
    }

    public void setRaw(Map raw) {
        this.raw = raw;
    }
}
