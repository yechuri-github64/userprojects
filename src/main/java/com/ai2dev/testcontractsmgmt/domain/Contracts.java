package com.ai2dev.testcontractsmgmt.domain;

import io.micronaut.data.annotation.Id;
import io.micronaut.data.annotation.MappedEntity;
import io.micronaut.data.annotation.GeneratedValue;

@MappedEntity("contracts")
public class Contracts {

    @Id
    @GeneratedValue
    private Long id;

    private String email;

    public Contracts() {
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }
}
