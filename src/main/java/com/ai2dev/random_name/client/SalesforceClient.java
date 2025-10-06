package com.ai2dev.random_name.client;

import com.ai2dev.random_name.model.dto.SalesforceAccountDto;
import io.micronaut.http.annotation.Get;
import io.micronaut.http.client.annotation.Client;
import io.micronaut.http.client.RxHttpClient;
import jakarta.inject.Named;

@Client("${salesforce.url}")
public interface SalesforceClient {

    // Example: get account by Salesforce Id
    @Get("/services/data/v52.0/sobjects/Account/{id}")
    SalesforceAccountDto getAccountById(String id);

    // Example: a custom endpoint that might search by email (this is illustrative)
    @Get("/services/data/v52.0/query/?q={soql}")
    SalesforceAccountDto getAccountByEmail(String emailEncodedSoql);
}
