package com.ai2dev.testjavaproject.repository;

import io.micronaut.data.annotation.Repository;
import io.micronaut.data.repository.CrudRepository;
import com.ai2dev.testjavaproject.model.Account;

@Repository
public interface AccountRepository extends CrudRepository<Account, Long> {
}
