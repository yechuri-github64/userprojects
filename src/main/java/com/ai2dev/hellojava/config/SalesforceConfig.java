package com.ai2dev.hellojava.config; import io.micronaut.context.annotation.ConfigurationProperties; @ConfigurationProperties ("salesforce") public class SalesforceConfig { private String clientId; private String clientSecret; private String username; private String password; public String getClientId () { return clientId; }

 public void setClientId (String clientId) { this.clientId = clientId; }

 public String getClientSecret () { return clientSecret; }

 public void setClientSecret (String clientSecret) { this.clientSecret = clientSecret; }

 public String getUsername () { return username; }

 public void setUsername (String username) { this.username = username; }

 public String getPassword () { return password; }

 public void setPassword (String password) { this.password = password; }
}