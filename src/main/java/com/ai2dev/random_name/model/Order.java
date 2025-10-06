package com.ai2dev.random_name.model;

import io.micronaut.data.annotation.MappedEntity;
import io.micronaut.data.annotation.Id;
import io.micronaut.data.annotation.GeneratedValue;
import io.micronaut.serde.annotation.Serdeable;

import java.math.BigDecimal;

@MappedEntity("orders")
@Serdeable
public class Order {
    @Id
    @GeneratedValue
    private Long id;
    private Long customerId;
    private BigDecimal total;

    public Order() {
    }

    public Order(Long id, Long customerId, BigDecimal total) {
        this.id = id;
        this.customerId = customerId;
        this.total = total;
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public Long getCustomerId() {
        return customerId;
    }

    public void setCustomerId(Long customerId) {
        this.customerId = customerId;
    }

    public BigDecimal getTotal() {
        return total;
    }

    public void setTotal(BigDecimal total) {
        this.total = total;
    }
}
