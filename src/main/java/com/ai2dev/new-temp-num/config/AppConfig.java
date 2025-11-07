package com.ai2dev.newtempnum.config;

import io.micronaut.context.annotation.ConfigurationProperties;

@ConfigurationProperties("app")
public class AppConfig {
  private String someProperty;

  public String getSomeProperty() {
    return someProperty;
  }

  public void setSomeProperty(String someProperty) {
    this.someProperty = someProperty;
  }
}