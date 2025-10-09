package com.ai2dev.testcontractsmgmt.controller;

import io.micronaut.http.annotation.Controller;
import io.micronaut.http.annotation.Get;
import jakarta.inject.Inject;
import java.util.List;
import com.ai2dev.testcontractsmgmt.dto.ContractInfoDto;
import com.ai2dev.testcontractsmgmt.service.ContractService;

@Controller("/contracts")
public class ContractController {

    private final ContractService contractService;

    @Inject
    public ContractController(ContractService contractService) {
        this.contractService = contractService;
    }

    @Get("/info")
    public List<ContractInfoDto> getContractInfo() {
        return contractService.listAll();
    }
}
