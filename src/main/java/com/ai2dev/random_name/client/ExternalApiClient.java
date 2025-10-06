package com.ai2dev.random_name.client;

import com.ai2dev.random_name.model.dto.ExternalDto;
import io.micronaut.http.annotation.Get;
import io.micronaut.http.client.annotation.Client;

@Client("${external-api.url}")
public interface ExternalApiClient {

    @Get("/data/{id}")
    ExternalDto fetchData(Long id);
}
