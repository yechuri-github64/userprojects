package com.ai2dev.random_name.model;

import io.micronaut.serde.annotation.Serdeable;

@Serdeable
public class AggregatedDataDto {
    private ExternalDataDto externalData;
    private SalesforceAccountDto salesforceAccount;
    private Iterable<Customer> customers;
    private Iterable<Order> orders;
    private Iterable<Product> products;

    public AggregatedDataDto() {
    }

    public AggregatedDataDto(ExternalDataDto externalData, SalesforceAccountDto salesforceAccount, Iterable<Customer> customers, Iterable<Order> orders, Iterable<Product> products) {
        this.externalData = externalData;
        this.salesforceAccount = salesforceAccount;
        this.customers = customers;
        this.orders = orders;
        this.products = products;
    }

    public ExternalDataDto getExternalData() {
        return externalData;
    }

    public void setExternalData(ExternalDataDto externalData) {
        this.externalData = externalData;
    }

    public SalesforceAccountDto getSalesforceAccount() {
        return salesforceAccount;
    }

    public void setSalesforceAccount(SalesforceAccountDto salesforceAccount) {
        this.salesforceAccount = salesforceAccount;
    }

    public Iterable<Customer> getCustomers() {
        return customers;
    }

    public void setCustomers(Iterable<Customer> customers) {
        this.customers = customers;
    }

    public Iterable<Order> getOrders() {
        return orders;
    }

    public void setOrders(Iterable<Order> orders) {
        this.orders = orders;
    }

    public Iterable<Product> getProducts() {
        return products;
    }

    public void setProducts(Iterable<Product> products) {
        this.products = products;
    }
}
