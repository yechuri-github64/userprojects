package com.ai2dev.test_project_java.repository;

import io.micronaut.data.annotation.Repository;
import io.micronaut.data.repository.CrudRepository;
import com.ai2dev.test_project_java.model.Account;

@Repository
public interface AccountRepository extends CrudRepository<Account, Long> {
}
