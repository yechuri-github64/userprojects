package com.ai2dev.random_name.model;

import io.micronaut.serde.annotation.Serdeable;

@Serdeable
public class SalesforceAccountDto {
    private String Id;
    private String Name;

    public SalesforceAccountDto() {
    }

    public SalesforceAccountDto(String id, String name) {
        this.Id = id;
        this.Name = name;
    }

    public String getId() {
        return Id;
    }

    public void setId(String id) {
        Id = id;
    }

    public String getName() {
        return Name;
    }

    public void setName(String name) {
        Name = name;
    }
}
