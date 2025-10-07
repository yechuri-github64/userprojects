package com.ai2dev.get_accounts.dto;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.Size;
import jakarta.validation.constraints.Digits;
import java.math.BigDecimal;

public class AccountUpdateRequest {

    @Size(max = 255)
    private String name;

    @Email
    @Size(max = 255)
    private String email;

    @Digits(integer = 15, fraction = 4)
    private BigDecimal balance;

    @Size(max = 50)
    private String status;

    public AccountUpdateRequest() {}

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public BigDecimal getBalance() {
        return balance;
    }

    public void setBalance(BigDecimal balance) {
        this.balance = balance;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }
}
