package com.ai2dev.random_name.repository;

import com.ai2dev.random_name.model.Order;
import io.micronaut.data.jdbc.annotation.JdbcRepository;
import io.micronaut.data.model.query.builder.sql.Dialect;
import io.micronaut.data.repository.CrudRepository;

@JdbcRepository(dialect = Dialect.SQL_SERVER, dataSource = "sqlserver")
public interface OrderRepository extends CrudRepository<Order, Long> {
}
