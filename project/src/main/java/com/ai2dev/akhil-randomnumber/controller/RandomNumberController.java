package com.ai2dev.akhilrandomnumber.controller;

import com.ai2dev.akhilrandomnumber.service.RandomNumberService;
import io.micronaut.http.HttpResponse;
import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Get;
import io.micronaut.http.annotation.QueryValue;
import javax.inject.Inject;

@Controller("/random")
public class RandomNumberController {
    @Inject
    private RandomNumberService randomNumberService;

    @Get
    public HttpResponse<?> getRandomNumber(@QueryValue String name) {
        try {
            int randomNumber = randomNumberService.generateRandomNumber();
            return HttpResponse.ok(Map.of("randomNumber", randomNumber));
        } catch (Exception e) {
            return HttpResponse.serverError(Map.of("error", e.getMessage()));
        }
    }
}