package com.ai2dev.random_name.model;

import io.micronaut.serde.annotation.Serdeable;

@Serdeable
public class ExternalDataDto {
    private String data;

    public ExternalDataDto() {
    }

    public ExternalDataDto(String data) {
        this.data = data;
    }

    public String getData() {
        return data;
    }

    public void setData(String data) {
        this.data = data;
    }
}
