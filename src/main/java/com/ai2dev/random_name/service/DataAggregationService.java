package com.ai2dev.random_name.service;

import com.ai2dev.random_name.model.AggregatedDataDto;
import com.ai2dev.random_name.model.Customer;
import com.ai2dev.random_name.model.ExternalDataDto;
import com.ai2dev.random_name.model.Order;
import com.ai2dev.random_name.model.Product;
import com.ai2dev.random_name.model.SalesforceAccountDto;
import com.ai2dev.random_name.repository.CustomerRepository;
import com.ai2dev.random_name.repository.OrderRepository;
import com.ai2dev.random_name.repository.ProductRepository;
import jakarta.inject.Singleton;

@Singleton
public class DataAggregationService {

    private final ExternalApiService externalApiService;
    private final SalesforceService salesforceService;
    private final CustomerRepository customerRepository;
    private final OrderRepository orderRepository;
    private final ProductRepository productRepository;

    public DataAggregationService(ExternalApiService externalApiService,
                                  SalesforceService salesforceService,
                                  CustomerRepository customerRepository,
                                  OrderRepository orderRepository,
                                  ProductRepository productRepository) {
        this.externalApiService = externalApiService;
        this.salesforceService = salesforceService;
        this.customerRepository = customerRepository;
        this.orderRepository = orderRepository;
        this.productRepository = productRepository;
    }

    public AggregatedDataDto aggregate(String externalParam, String sfAccountId) {
        ExternalDataDto external = externalApiService.getExternalData(externalParam);
        SalesforceAccountDto account = salesforceService.getAccount(sfAccountId);
        Iterable<Customer> customers = customerRepository.findAll();
        Iterable<Order> orders = orderRepository.findAll();
        Iterable<Product> products = productRepository.findAll();
        return new AggregatedDataDto(external, account, customers, orders, products);
    }
}
