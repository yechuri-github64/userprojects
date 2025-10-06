package com.ai2dev.random_name.repository;

import com.ai2dev.random_name.model.Product;
import io.micronaut.data.jdbc.annotation.JdbcRepository;
import io.micronaut.data.model.query.builder.sql.Dialect;
import io.micronaut.data.repository.CrudRepository;

@JdbcRepository(dialect = Dialect.ORACLE, dataSource = "oracle")
public interface ProductRepository extends CrudRepository<Product, Long> {
}
