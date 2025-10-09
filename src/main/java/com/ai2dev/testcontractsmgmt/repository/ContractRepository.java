package com.ai2dev.testcontractsmgmt.repository;

import io.micronaut.data.annotation.Query;
import io.micronaut.data.jdbc.annotation.JdbcRepository;
import io.micronaut.data.model.query.builder.sql.Dialect;
import io.micronaut.data.repository.CrudRepository;
import java.util.List;
import com.ai2dev.testcontractsmgmt.dto.ContractInfoDto;
import com.ai2dev.testcontractsmgmt.domain.Contracts;

@JdbcRepository(dialect = Dialect.MYSQL)
public interface ContractRepository extends CrudRepository<Contracts, Long> {

    @Query("SELECT c.email AS email, o.contractinformation AS contractInformation FROM contracts c JOIN contract_orders o ON o.contractsId = c.id ORDER BY c.email")
    List<ContractInfoDto> findAllContractInfo();
}
