package com.ai2dev.testcontractsmgmt.domain;

import io.micronaut.data.annotation.Id;
import io.micronaut.data.annotation.MappedEntity;
import io.micronaut.data.annotation.GeneratedValue;
import io.micronaut.data.annotation.MappedProperty;

@MappedEntity("contract_orders")
public class ContractOrder {

    @Id
    @GeneratedValue
    private Long id;

    @MappedProperty("contractinformation")
    private String contractInformation;

    @MappedProperty("contractsId")
    private Long contractsId;

    public ContractOrder() {
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getContractInformation() {
        return contractInformation;
    }

    public void setContractInformation(String contractInformation) {
        this.contractInformation = contractInformation;
    }

    public Long getContractsId() {
        return contractsId;
    }

    public void setContractsId(Long contractsId) {
        this.contractsId = contractsId;
    }
}
