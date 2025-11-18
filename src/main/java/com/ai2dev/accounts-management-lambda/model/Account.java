package com.ai2dev.accountsmanagementlambda.model; import io.micronaut.data.annotation.Id; import io.micronaut.data.annotation.MappedEntity; @MappedEntity
public class Account { @Id
 private Long id; private String name; private String email; private String address; public Account (Long id, String name, String email, String address) { this.id = id; this.name = name; this.email = email; this.address = address; }

 public Long getId () { return id; }

 public void setId (Long id) { this.id = id; }

 public String getName () { return name; }

 public String getEmail () { return email; }

 public String getAddress () { return address; }
}