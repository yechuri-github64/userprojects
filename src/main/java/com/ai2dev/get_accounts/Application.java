package com.ai2dev.get_accounts;

import io.micronaut.runtime.Micronaut;

public class Application {
    public static void main(String[] args) {
        Micronaut.build(args)
                .packages("com.ai2dev.get_accounts")
                .mainClass(Application.class)
                .start();
    }
}
