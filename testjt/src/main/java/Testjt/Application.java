package Testjt;

import io.micronaut.runtime.Micronaut;
import io.micronaut.context.annotation.*;
import com.amazonaws.*;

public class Application {
    public static void main(String[] args) {
        Micronaut.run(Application.class, args);
    }
}
