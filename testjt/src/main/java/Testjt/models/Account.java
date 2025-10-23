package Testjt.models;

import java.util.Map;
import java.util.HashMap;
import io.micronaut.core.annotation.Introspected;

@Introspected
public class Account {
    private Long id;
    private String name;
    private String email;
    private String createdAt;

    public Account() {}

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

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

    public String getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(String createdAt) {
        this.createdAt = createdAt;
    }

    public Map<String, Object> toMap() {
        Map<String, Object> m = new HashMap<>();
        m.put("id", id);
        m.put("name", name);
        m.put("email", email);
        m.put("createdAt", createdAt);
        return m;
    }
}
