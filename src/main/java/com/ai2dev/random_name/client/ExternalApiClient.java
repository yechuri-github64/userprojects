package com.ai2dev.random_name.client;

import com.ai2dev.random_name.model.ExternalDataDto;
import io.micronaut.http.annotation.Get;
import io.micronaut.http.annotation.QueryValue;
import io.micronaut.http.client.annotation.Client;

@Client("${external.api.base-url}")
public interface ExternalApiClient {
    @Get("/data{?param}")
    ExternalDataDto getData(@QueryValue String param);
}
