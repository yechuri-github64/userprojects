package com.ai2dev.accounts_salesforce_app.client;

import com.ai2dev.accounts_salesforce_app.config.SalesforceConfiguration;
import com.ai2dev.accounts_salesforce_app.model.Account;
import io.micronaut.http.HttpRequest;
import io.micronaut.http.HttpResponse;
import io.micronaut.http.MediaType;
import io.micronaut.http.client.HttpClient;
import io.micronaut.http.client.BlockingHttpClient;
import jakarta.inject.Singleton;

import java.net.URI;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@Singleton
public class SalesforceClient {
    private final SalesforceConfiguration config;
    private final BlockingHttpClient client;
    private String accessToken;

    public SalesforceClient(SalesforceConfiguration config) {
        this.config = config;
        this.client = HttpClient.create(URI.create(config.getBaseUrl())).toBlocking();
    }

    private synchronized String getAccessToken() {
        if (accessToken != null) return accessToken;
        Map<String, Object> body = new HashMap<>();
        body.put("grant_type", "password");
        body.put("client_id", config.getClientId());
        body.put("client_secret", config.getClientSecret());
        body.put("username", config.getUsername());
        body.put("password", config.getPassword());

        HttpRequest<Map<String, Object>> req = HttpRequest.POST(config.getAuthUrl(), body)
                .contentType(MediaType.APPLICATION_FORM_URLENCODED_TYPE);
        HttpResponse<Map> resp = client.exchange(req, Map.class);
        if (resp != null && resp.getBody().isPresent()) {
            Map m = resp.getBody().get();
            Object token = m.get("access_token");
            if (token != null) {
                accessToken = token.toString();
            }
        }
        return accessToken;
    }

    public List<Map<String, Object>> createAccounts(List<Account> accounts) {
        String token = getAccessToken();
        if (token == null) throw new RuntimeException("Unable to authenticate to Salesforce");
        List<Map<String, Object>> created = new ArrayList<>();
        for (Account a : accounts) {
            Map<String, Object> payload = new HashMap<>();
            payload.put("Name", a.getName());
            payload.put("Phone", a.getPhone());
            payload.put("Website", a.getWebsite());
            HttpRequest<Map<String, Object>> req = HttpRequest.POST(config.getBaseUrl() + "/services/data/" + config.getApiVersion() + "/sobjects/Account", payload)
                    .header("Authorization", "Bearer " + token)
                    .contentType(MediaType.APPLICATION_JSON_TYPE);
            HttpResponse<Map> resp = client.exchange(req, Map.class);
            if (resp != null && resp.getBody().isPresent()) {
                created.add(resp.getBody().get());
            } else {
                Map<String, Object> failure = new HashMap<>();
                failure.put("success", false);
                failure.put("message", "no response from salesforce");
                created.add(failure);
            }
        }
        return created;
    }

    public Map getAccount(String id) {
        String token = getAccessToken();
        HttpRequest<?> req = HttpRequest.GET(config.getBaseUrl() + "/services/data/" + config.getApiVersion() + "/sobjects/Account/" + id)
                .header("Authorization", "Bearer " + token);
        HttpResponse<Map> resp = client.exchange(req, Map.class);
        return resp.getBody().orElse(null);
    }

    public Map updateAccount(String id, Map<String, Object> updates) {
        String token = getAccessToken();
        HttpRequest<Map<String, Object>> req = HttpRequest.PATCH(config.getBaseUrl() + "/services/data/" + config.getApiVersion() + "/sobjects/Account/" + id, updates)
                .header("Authorization", "Bearer " + token)
                .contentType(MediaType.APPLICATION_JSON_TYPE);
        HttpResponse<Map> resp = client.exchange(req, Map.class);
        return resp.getBody().orElse(null);
    }

    public boolean deleteAccount(String id) {
        String token = getAccessToken();
        HttpRequest<?> req = HttpRequest.DELETE(config.getBaseUrl() + "/services/data/" + config.getApiVersion() + "/sobjects/Account/" + id)
                .header("Authorization", "Bearer " + token);
        HttpResponse<?> resp = client.exchange(req);
        return resp.getStatus().getCode() >= 200 && resp.getStatus().getCode() < 300;
    }
}
