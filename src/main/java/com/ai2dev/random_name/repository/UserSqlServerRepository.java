package com.ai2dev.random_name.repository;

import com.ai2dev.random_name.model.User;
import io.micronaut.data.annotation.Repository;
import io.micronaut.data.jdbc.annotation.JdbcRepository;
import io.micronaut.data.model.query.builder.sql.Dialect;
import io.micronaut.data.repository.CrudRepository;

@JdbcRepository(dialect = Dialect.SQL_SERVER)
@Repository
public interface UserSqlServerRepository extends CrudRepository<User, Long> {
}
