package com.ai2dev.get_accounts.error;

import io.micronaut.http.HttpRequest;
import io.micronaut.http.HttpResponse;
import io.micronaut.http.HttpStatus;
import io.micronaut.http.MediaType;
import io.micronaut.http.annotation.Produces;
import io.micronaut.http.server.exceptions.ExceptionHandler;
import jakarta.inject.Singleton;
import jakarta.validation.ConstraintViolation;
import jakarta.validation.ConstraintViolationException;

import java.time.Instant;
import java.util.NoSuchElementException;
import java.util.stream.Collectors;

@Produces(MediaType.APPLICATION_JSON)
@Singleton
public class GlobalExceptionHandler implements ExceptionHandler<Exception, HttpResponse<ApiError>> {

    @Override
    public HttpResponse<ApiError> handle(HttpRequest request, Exception exception) {
        if (exception instanceof ConstraintViolationException cve) {
            String message = cve.getConstraintViolations().stream()
                    .map(ConstraintViolation::getMessage)
                    .collect(Collectors.joining(", "));
            return build(HttpStatus.BAD_REQUEST, request, message);
        }
        if (exception instanceof IllegalArgumentException iae) {
            return build(HttpStatus.BAD_REQUEST, request, iae.getMessage());
        }
        if (exception instanceof NoSuchElementException nse) {
            return build(HttpStatus.NOT_FOUND, request, nse.getMessage());
        }
        return build(HttpStatus.INTERNAL_SERVER_ERROR, request, "Unexpected error");
    }

    private HttpResponse<ApiError> build(HttpStatus status, HttpRequest<?> request, String message) {
        ApiError err = new ApiError(Instant.now(), status.getCode(), status.getReason(), message, request.getPath());
        return HttpResponse.status(status).body(err);
    }
}
