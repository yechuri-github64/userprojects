package com.ai2dev.random_name.client;

import com.ai2dev.random_name.model.SalesforceAccountDto;
import io.micronaut.http.annotation.Get;
import io.micronaut.http.annotation.Header;
import io.micronaut.http.annotation.PathVariable;
import io.micronaut.http.client.annotation.Client;

@Client("${salesforce.api.api-url}")
public interface SalesforceApiClient {
    @Get("/{version}/sobjects/Account/{id}")
    SalesforceAccountDto getAccount(@Header("Authorization") String authorization,
                                    @PathVariable String version,
                                    @PathVariable String id);
}
