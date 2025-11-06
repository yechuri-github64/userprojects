package com.ai2dev.akhil_randomnumber.repository;

import javax.inject.Singleton;
import com.ai2dev.akhil_randomnumber.model.RandomNumberRecord;
import java.util.ArrayList;
import java.util.List;

@Singleton
public class RandomNumberRepository {
    private final List<RandomNumberRecord> storage = new ArrayList<>();

    public void save(RandomNumberRecord record) {
        storage.add(record);
    }

    public List<RandomNumberRecord> findAll() {
        return new ArrayList<>(storage);
    }
}
