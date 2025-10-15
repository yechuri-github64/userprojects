package com.ai2dev.test_project_java.service;

import java.util.List;
import java.util.Optional;
import com.ai2dev.test_project_java.model.Account;
import com.ai2dev.test_project_java.dto.AccountDto;

public interface AccountService {
    List<Account> getAll();
    Optional<Account> getById(Long id);
    java.util.List<Account> createMultiple(java.util.List<AccountDto> dtos);
    Account update(Long id, AccountDto dto);
    void delete(Long id);
}
