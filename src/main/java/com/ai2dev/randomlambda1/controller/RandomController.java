package com.ai2dev.randomlambda1.controller;

import com.ai2dev.randomlambda1.service.RandomService;
import com.ai2dev.randomlambda1.model.RandomResponse;
import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Get;
import io.micronaut.http.annotation.PathVariable;
import jakarta.inject.Inject;

@Controller("/random")
public class RandomController {

    @Inject
    private RandomService randomService;

    @Get("/")
    public RandomResponse randomAbove100() {
        return randomService.generateAbove(100);
    }

    @Get("/above/{min}")
    public RandomResponse randomAboveMin(@PathVariable int min) {
        return randomService.generateAbove(Math.max(min, 100));
    }
}
