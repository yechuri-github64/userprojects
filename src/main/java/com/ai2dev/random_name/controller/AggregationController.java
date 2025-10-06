package com.ai2dev.random_name.controller;

import com.ai2dev.random_name.model.AggregatedDataDto;
import com.ai2dev.random_name.model.ExternalDataDto;
import com.ai2dev.random_name.model.SalesforceAccountDto;
import com.ai2dev.random_name.model.Customer;
import com.ai2dev.random_name.model.Order;
import com.ai2dev.random_name.model.Product;
import com.ai2dev.random_name.repository.CustomerRepository;
import com.ai2dev.random_name.repository.OrderRepository;
import com.ai2dev.random_name.repository.ProductRepository;
import com.ai2dev.random_name.service.DataAggregationService;
import com.ai2dev.random_name.service.ExternalApiService;
import com.ai2dev.random_name.service.SalesforceService;
import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Get;
import io.micronaut.http.annotation.PathVariable;
import io.micronaut.http.annotation.QueryValue;
import jakarta.inject.Singleton;

@Controller("/api")
public class AggregationController {

    private final DataAggregationService aggregationService;
    private final ExternalApiService externalApiService;
    private final SalesforceService salesforceService;
    private final CustomerRepository customerRepository;
    private final OrderRepository orderRepository;
    private final ProductRepository productRepository;

    public AggregationController(DataAggregationService aggregationService,
                                 ExternalApiService externalApiService,
                                 SalesforceService salesforceService,
                                 CustomerRepository customerRepository,
                                 OrderRepository orderRepository,
                                 ProductRepository productRepository) {
        this.aggregationService = aggregationService;
        this.externalApiService = externalApiService;
        this.salesforceService = salesforceService;
        this.customerRepository = customerRepository;
        this.orderRepository = orderRepository;
        this.productRepository = productRepository;
    }

    @Get("/aggregate")
    public AggregatedDataDto aggregate(@QueryValue String externalParam, @QueryValue String sfAccountId) {
        return aggregationService.aggregate(externalParam, sfAccountId);
    }

    @Get("/external")
    public ExternalDataDto external(@QueryValue String param) {
        return externalApiService.getExternalData(param);
    }

    @Get("/salesforce/account/{id}")
    public SalesforceAccountDto getSalesforceAccount(@PathVariable String id) {
        return salesforceService.getAccount(id);
    }

    @Get("/customers")
    public Iterable<Customer> customers() {
        return customerRepository.findAll();
    }

    @Get("/orders")
    public Iterable<Order> orders() {
        return orderRepository.findAll();
    }

    @Get("/products")
    public Iterable<Product> products() {
        return productRepository.findAll();
    }
}
