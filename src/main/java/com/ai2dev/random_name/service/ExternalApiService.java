package com.ai2dev.random_name.service;

import com.ai2dev.random_name.client.ExternalApiClient;
import com.ai2dev.random_name.model.ExternalDataDto;
import jakarta.inject.Singleton;

@Singleton
public class ExternalApiService {

    private final ExternalApiClient externalApiClient;

    public ExternalApiService(ExternalApiClient externalApiClient) {
        this.externalApiClient = externalApiClient;
    }

    public ExternalDataDto getExternalData(String param) {
        return externalApiClient.getData(param);
    }
}
