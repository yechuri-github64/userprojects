package com.ai2dev.akhil_randomnumber.model;

import java.time.Instant;

public class RandomNumberRecord {
    private int number;
    private Instant createdAt;

    public RandomNumberRecord() {}

    public RandomNumberRecord(int number) {
        this.number = number;
        this.createdAt = Instant.now();
    }

    public int getNumber() { return number; }
    public void setNumber(int number) { this.number = number; }
    public Instant getCreatedAt() { return createdAt; }
    public void setCreatedAt(Instant createdAt) { this.createdAt = createdAt; }
}
