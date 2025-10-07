package com.ai2dev.guess_name.application.service;

import com.ai2dev.guess_name.domain.audit.AuditLog;
import com.ai2dev.guess_name.domain.audit.AuditLogRepository;
import io.micronaut.transaction.annotation.TransactionalAdvice;
import jakarta.inject.Singleton;
import java.time.Instant;

@Singleton
public class AuditService {

    private final AuditLogRepository auditLogRepository;

    public AuditService(AuditLogRepository auditLogRepository) {
        this.auditLogRepository = auditLogRepository;
    }

    @TransactionalAdvice("audit")
    public void logAction(String action) {
        AuditLog log = new AuditLog();
        log.setAction(action);
        log.setCreatedAt(Instant.now());
        auditLogRepository.save(log);
    }
}
