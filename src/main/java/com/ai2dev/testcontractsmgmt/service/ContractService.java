package com.ai2dev.testcontractsmgmt.service;

import jakarta.inject.Singleton;
import java.util.List;
import com.ai2dev.testcontractsmgmt.dto.ContractInfoDto;
import com.ai2dev.testcontractsmgmt.repository.ContractRepository;

@Singleton
public class ContractService {

    private final ContractRepository repository;

    public ContractService(ContractRepository repository) {
        this.repository = repository;
    }

    public List<ContractInfoDto> listAll() {
        return repository.findAllContractInfo();
    }
}
