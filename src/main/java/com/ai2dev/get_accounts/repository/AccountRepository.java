package com.ai2dev.get_accounts.repository;

import com.ai2dev.get_accounts.domain.Account;
import io.micronaut.data.annotation.Repository;
import io.micronaut.data.jpa.repository.JpaRepository;

@Repository("mysql")
public interface AccountRepository extends JpaRepository<Account, Long> {
}
