package com.ai2dev.guess_name.api.controller;

import com.ai2dev.guess_name.api.dto.GuessRequest;
import com.ai2dev.guess_name.api.dto.GuessResponse;
import com.ai2dev.guess_name.application.service.GuessService;
import io.micronaut.http.HttpResponse;
import io.micronaut.http.annotation.Body;
import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Get;
import io.micronaut.http.annotation.PathVariable;
import io.micronaut.http.annotation.Post;
import jakarta.validation.Valid;

import java.util.List;

@Controller("/api/v1/guesses")
public class GuessController {

    private final GuessService guessService;

    public GuessController(GuessService guessService) {
        this.guessService = guessService;
    }

    @Post
    public HttpResponse<GuessResponse> create(@Body @Valid GuessRequest request) {
        return HttpResponse.created(guessService.createGuess(request));
    }

    @Get("/{id}")
    public GuessResponse get(@PathVariable Long id) {
        return guessService.getGuess(id);
    }

    @Get
    public List<GuessResponse> list() {
        return guessService.listGuesses();
    }
}
