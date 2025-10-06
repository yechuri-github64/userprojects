package com.ai2dev.random_name.repository;

import com.ai2dev.random_name.model.Customer;
import io.micronaut.data.annotation.Repository;
import io.micronaut.data.jdbc.annotation.JdbcRepository;
import io.micronaut.data.model.query.builder.sql.Dialect;
import io.micronaut.data.repository.CrudRepository;

@JdbcRepository(dialect = Dialect.MYSQL, dataSource = "mysql")
public interface CustomerRepository extends CrudRepository<Customer, Long> {
}
