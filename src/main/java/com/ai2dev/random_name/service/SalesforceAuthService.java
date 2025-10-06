package com.ai2dev.random_name.service;

import io.micronaut.context.annotation.Value;
import io.micronaut.core.type.Argument;
import io.micronaut.http.HttpRequest;
import io.micronaut.http.MediaType;
import io.micronaut.http.client.HttpClient;
import io.micronaut.http.client.BlockingHttpClient;
import io.micronaut.json.JsonMapper;
import jakarta.inject.Singleton;

import java.net.MalformedURLException;
import java.net.URL;
import java.nio.charset.StandardCharsets;
import java.util.Map;

@Singleton
public class SalesforceAuthService {

    @Value("${salesforce.auth.token-url}")
    protected String tokenUrl;

    @Value("${salesforce.auth.client-id}")
    protected String clientId;

    @Value("${salesforce.auth.client-secret}")
    protected String clientSecret;

    @Value("${salesforce.auth.username}")
    protected String username;

    @Value("${salesforce.auth.password}")
    protected String password;

    private final JsonMapper jsonMapper;

    public SalesforceAuthService(JsonMapper jsonMapper) {
        this.jsonMapper = jsonMapper;
    }

    public String fetchAccessToken() {
        try {
            URL url = new URL(tokenUrl);
            HttpClient client = HttpClient.create(url);
            BlockingHttpClient blocking = client.toBlocking();
            String form = "grant_type=password" +
                    "&client_id=" + encode(clientId) +
                    "&client_secret=" + encode(clientSecret) +
                    "&username=" + encode(username) +
                    "&password=" + encode(password);

            HttpRequest<String> request = HttpRequest.POST(url.toString(), form)
                    .contentType(MediaType.APPLICATION_FORM_URLENCODED_TYPE)
                    .accept(MediaType.APPLICATION_JSON_TYPE);

            String json = blocking.retrieve(request);
            Map<String, Object> map = jsonMapper.readValue(json.getBytes(StandardCharsets.UTF_8), Argument.mapOf(String.class, Object.class));
            Object token = map.get("access_token");
            return token == null ? "" : token.toString();
        } catch (MalformedURLException e) {
            throw new IllegalStateException("Invalid Salesforce token URL", e);
        } catch (Exception e) {
            throw new IllegalStateException("Failed to obtain Salesforce access token", e);
        }
    }

    private static String encode(String v) {
        if (v == null) return "";
        return v.replace("%", "%25").replace(" ", "+").replace("&", "%26").replace("=", "%3D");
    }
}
