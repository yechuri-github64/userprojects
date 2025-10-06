package com.ai2dev.random_name.model.dto;

import com.ai2dev.random_name.model.User;

public class AggregatedDataDto {
    private User user;
    private ExternalDto external;
    private SalesforceAccountDto salesforceAccount;

    public AggregatedDataDto() {
    }

    public User getUser() {
        return user;
    }

    public void setUser(User user) {
        this.user = user;
    }

    public ExternalDto getExternal() {
        return external;
    }

    public void setExternal(ExternalDto external) {
        this.external = external;
    }

    public SalesforceAccountDto getSalesforceAccount() {
        return salesforceAccount;
    }

    public void setSalesforceAccount(SalesforceAccountDto salesforceAccount) {
        this.salesforceAccount = salesforceAccount;
    }
}
