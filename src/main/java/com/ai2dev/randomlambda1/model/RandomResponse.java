package com.ai2dev.randomlambda1.model;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonProperty;

public class RandomResponse {
    private final int value;
    private final String timestamp;

    @JsonCreator
    public RandomResponse(@JsonProperty("value") int value, @JsonProperty("timestamp") String timestamp) {
        this.value = value;
        this.timestamp = timestamp;
    }

    public int getValue() {
        return value;
    }

    public String getTimestamp() {
        return timestamp;
    }
}
