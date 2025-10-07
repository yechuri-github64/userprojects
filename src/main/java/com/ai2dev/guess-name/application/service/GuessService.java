package com.ai2dev.guess_name.application.service;

import com.ai2dev.guess_name.api.dto.GuessRequest;
import com.ai2dev.guess_name.api.dto.GuessResponse;
import com.ai2dev.guess_name.domain.main.NameGuess;
import com.ai2dev.guess_name.domain.main.NameGuessRepository;
import io.micronaut.transaction.annotation.TransactionalAdvice;

import jakarta.inject.Singleton;
import jakarta.validation.Valid;
import java.time.Instant;
import java.util.List;
import java.util.stream.Collectors;

@Singleton
public class GuessService {

    private final NameGuessRepository nameGuessRepository;
    private final AuditService auditService;

    public GuessService(NameGuessRepository nameGuessRepository, AuditService auditService) {
        this.nameGuessRepository = nameGuessRepository;
        this.auditService = auditService;
    }

    @TransactionalAdvice("default")
    public GuessResponse createGuess(@Valid GuessRequest request) {
        NameGuess entity = new NameGuess();
        entity.setGuessedName(request.getName());
        entity.setCreatedAt(Instant.now());
        entity = nameGuessRepository.save(entity);
        auditService.logAction("Created guess with id " + entity.getId());
        return toResponse(entity);
    }

    @TransactionalAdvice("default")
    public GuessResponse getGuess(Long id) {
        NameGuess entity = nameGuessRepository.findById(id)
                .orElseThrow(() -> new IllegalArgumentException("Guess not found"));
        return toResponse(entity);
    }

    @TransactionalAdvice("default")
    public List<GuessResponse> listGuesses() {
        return nameGuessRepository.findAll().stream()
                .map(this::toResponse)
                .collect(Collectors.toList());
    }

    private GuessResponse toResponse(NameGuess e) {
        GuessResponse r = new GuessResponse();
        r.setId(e.getId());
        r.setName(e.getGuessedName());
        r.setCreatedAt(e.getCreatedAt());
        return r;
    }
}
