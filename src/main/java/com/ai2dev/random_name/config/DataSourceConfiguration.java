package com.ai2dev.random_name.config;

import io.micronaut.context.annotation.Factory;
import io.micronaut.context.annotation.Requires;
import jakarta.inject.Singleton;
import javax.sql.DataSource;
import io.micronaut.context.annotation.Property;
import io.micronaut.context.annotation.Value;
import com.zaxxer.hikari.HikariConfig;
import com.zaxxer.hikari.HikariDataSource;

@Factory
public class DataSourceConfiguration {

    @Singleton
    @Requires(property = "datasources.mysql.url")
    public DataSource mysqlDataSource(@Value("${datasources.mysql.url}") String url,
                                      @Value("${datasources.mysql.username}") String username,
                                      @Value("${datasources.mysql.password}") String password,
                                      @Value("${datasources.mysql.driverClassName:com.mysql.cj.jdbc.Driver}") String driver) {
        HikariConfig cfg = new HikariConfig();
        cfg.setJdbcUrl(url);
        cfg.setUsername(username);
        cfg.setPassword(password);
        cfg.setDriverClassName(driver);
        cfg.setPoolName("mysql-pool");
        return new HikariDataSource(cfg);
    }

    @Singleton
    @Requires(property = "datasources.sqlserver.url")
    public DataSource sqlServerDataSource(@Value("${datasources.sqlserver.url}") String url,
                                          @Value("${datasources.sqlserver.username}") String username,
                                          @Value("${datasources.sqlserver.password}") String password,
                                          @Value("${datasources.sqlserver.driverClassName:com.microsoft.sqlserver.jdbc.SQLServerDriver}") String driver) {
        HikariConfig cfg = new HikariConfig();
        cfg.setJdbcUrl(url);
        cfg.setUsername(username);
        cfg.setPassword(password);
        cfg.setDriverClassName(driver);
        cfg.setPoolName("sqlserver-pool");
        return new HikariDataSource(cfg);
    }

    @Singleton
    @Requires(property = "datasources.oracle.url")
    public DataSource oracleDataSource(@Value("${datasources.oracle.url}") String url,
                                       @Value("${datasources.oracle.username}") String username,
                                       @Value("${datasources.oracle.password}") String password,
                                       @Value("${datasources.oracle.driverClassName:oracle.jdbc.OracleDriver}") String driver) {
        HikariConfig cfg = new HikariConfig();
        cfg.setJdbcUrl(url);
        cfg.setUsername(username);
        cfg.setPassword(password);
        cfg.setDriverClassName(driver);
        cfg.setPoolName("oracle-pool");
        return new HikariDataSource(cfg);
    }
}
