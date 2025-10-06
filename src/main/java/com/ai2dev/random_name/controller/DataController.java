package com.ai2dev.random_name.controller;

import com.ai2dev.random_name.model.dto.AggregatedDataDto;
import com.ai2dev.random_name.service.DataService;
import io.micronaut.http.HttpResponse;
import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Get;
import jakarta.inject.Inject;

@Controller("/api/v1/data")
public class DataController {

    @Inject
    DataService dataService;

    @Get("/{id}")
    public HttpResponse<AggregatedDataDto> getAggregatedData(Long id) {
        AggregatedDataDto dto = dataService.getAggregatedData(id);
        if (dto == null) {
            return HttpResponse.notFound();
        }
        return HttpResponse.ok(dto);
    }
}
