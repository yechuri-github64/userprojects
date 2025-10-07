package com.ai2dev.guess_name.domain.audit;

import io.micronaut.data.annotation.Repository;
import io.micronaut.data.jpa.repository.JpaRepository;

@Repository("audit")
public interface AuditLogRepository extends JpaRepository<AuditLog, Long> {
}
