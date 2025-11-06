package com.ai2dev.akhil_randomnumber.config;

import io.micronaut.context.annotation.ConfigurationProperties;

@ConfigurationProperties("datasources.mongodb")
public class MongoConfig {
    private String uri;

    public String getUri() { return uri; }
    public void setUri(String uri) { this.uri = uri; }
}
