package com.ai2dev.akhil_randomnumber.controller;

import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Get;
import javax.inject.Inject;
import com.ai2dev.akhil_randomnumber.service.RandomNumberService;
import java.util.Collections;
import java.util.Map;

@Controller("/random")
public class RandomNumberController {

    @Inject
    private RandomNumberService service;

    @Get("/generate")
    public Map<String, Integer> generate() {
        int num = service.generateGreaterThan500AndLessThan1000();
        return Collections.singletonMap("number", num);
    }
}
