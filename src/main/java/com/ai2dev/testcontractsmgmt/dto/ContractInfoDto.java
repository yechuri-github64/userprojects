package com.ai2dev.testcontractsmgmt.dto;

public class ContractInfoDto {

    private String email;
    private String contractInformation;

    public ContractInfoDto(String email, String contractInformation) {
        this.email = email;
        this.contractInformation = contractInformation;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getContractInformation() {
        return contractInformation;
    }

    public void setContractInformation(String contractInformation) {
        this.contractInformation = contractInformation;
    }
}
