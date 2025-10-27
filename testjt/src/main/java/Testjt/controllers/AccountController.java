package Testjt.controllers;

import Testjt.models.Account;
import Testjt.services.AccountService;
import io.micronaut.http.annotation.*;
import jakarta.inject.Inject;
import io.micronaut.http.HttpResponse;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

@Controller("/accounts")
public class AccountController {
    private static final Logger LOG = LoggerFactory.getLogger(AccountController.class);

    @Inject
    AccountService accountService;

    @Get("/last")
    public HttpResponse<?> getLast() {
        try {
            Account acc = accountService.getLastAccount();
            if (acc == null) {
                return HttpResponse.notFound(java.util.Map.of("error", java.util.Map.of("message", "No account found")));
            }
            return HttpResponse.ok(acc);
        } catch (Exception e) {
            LOG.error("Error fetching last account", e);
            return HttpResponse.serverError(java.util.Map.of("error", java.util.Map.of("message", e.getMessage(), "type", e.getClass().getName())));
        }
    }
}
