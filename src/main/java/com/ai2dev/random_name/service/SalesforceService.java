package com.ai2dev.random_name.service;

import com.ai2dev.random_name.client.SalesforceApiClient;
import com.ai2dev.random_name.model.SalesforceAccountDto;
import io.micronaut.context.annotation.Value;
import jakarta.inject.Singleton;

@Singleton
public class SalesforceService {

    private final SalesforceApiClient salesforceApiClient;
    private final SalesforceAuthService salesforceAuthService;

    @Value("${salesforce.api.api-version}")
    protected String apiVersion;

    public SalesforceService(SalesforceApiClient salesforceApiClient,
                             SalesforceAuthService salesforceAuthService) {
        this.salesforceApiClient = salesforceApiClient;
        this.salesforceAuthService = salesforceAuthService;
    }

    public SalesforceAccountDto getAccount(String accountId) {
        String token = salesforceAuthService.fetchAccessToken();
        String authHeader = "Bearer " + token;
        return salesforceApiClient.getAccount(authHeader, apiVersion, accountId);
    }
}
