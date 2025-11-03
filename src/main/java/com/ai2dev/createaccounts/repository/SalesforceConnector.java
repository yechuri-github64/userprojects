package com.ai2dev.createaccounts.repository;

import com.ai2dev.createaccounts.config.SalesforceConfiguration;
import com.ai2dev.createaccounts.model.Account;
import io.micronaut.http.HttpRequest;
import io.micronaut.http.MediaType;
import io.micronaut.http.client.annotation.Client;
import io.micronaut.http.client.HttpClient;
import jakarta.inject.Inject;
import jakarta.inject.Singleton;

import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.util.Map;

@Singleton
public class SalesforceConnector {

    @Inject
    private SalesforceConfiguration config;

    @Inject
    @Client("${salesforce.base-url}")
    private HttpClient httpClient;

    @SuppressWarnings("unchecked")
    public Account createAccount(Account account) {
        try {
            // Authenticate using OAuth2 password grant to obtain access token
            String form = "grant_type=password"
                    + "&client_id=" + URLEncoder.encode(config.getClientId(), StandardCharsets.UTF_8.toString())
                    + "&client_secret=" + URLEncoder.encode(config.getClientSecret(), StandardCharsets.UTF_8.toString())
                    + "&username=" + URLEncoder.encode(config.getUsername(), StandardCharsets.UTF_8.toString())
                    + "&password=" + URLEncoder.encode(config.getPassword(), StandardCharsets.UTF_8.toString());

            HttpRequest<String> tokenRequest = HttpRequest.POST("/services/oauth2/token", form)
                    .contentType(MediaType.APPLICATION_FORM_URLENCODED_TYPE)
                    .accept(MediaType.APPLICATION_JSON);

            Map<String, Object> tokenResponse = httpClient.toBlocking().retrieve(tokenRequest, Map.class);

            String accessToken = (String) tokenResponse.get("access_token");
            String instanceUrl = tokenResponse.containsKey("instance_url") ? (String) tokenResponse.get("instance_url") : config.getBaseUrl();

            // Create Account in Salesforce
            String createUrl = instanceUrl + "/services/data/v" + config.getApiVersion() + "/sobjects/Account";

            HttpRequest<Account> createRequest = HttpRequest.POST(createUrl, account)
                    .contentType(MediaType.APPLICATION_JSON_TYPE)
                    .header("Authorization", "Bearer " + accessToken)
                    .accept(MediaType.APPLICATION_JSON);

            Map<String, Object> createResponse = httpClient.toBlocking().retrieve(createRequest, Map.class);

            if (createResponse != null && createResponse.get("id") != null) {
                account.setId((String) createResponse.get("id"));
            }

            return account;

        } catch (Exception e) {
            throw new RuntimeException("Error creating account in Salesforce: " + e.getMessage(), e);
        }
    }
}
