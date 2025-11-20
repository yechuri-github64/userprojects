package com.ai2dev.accountsmanagementlambda.repository;

import com.ai2dev.accountsmanagementlambda.model.Account;
import io.micronaut.data.annotation.Repository;
import io.micronaut.data.repository.jpa.JpaRepository;

@Repository
public interface AccountRepositoryImpl extends JpaRepository<Account, Long> {

}