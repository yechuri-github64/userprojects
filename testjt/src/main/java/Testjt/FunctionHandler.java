package Testjt;

import com.amazonaws.services.lambda.runtime.Context;
import com.amazonaws.services.lambda.runtime.RequestHandler;
import io.micronaut.context.ApplicationContext;
import io.micronaut.context.annotation.*;
import io.micronaut.runtime.Micronaut;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import java.util.Map;
import java.util.HashMap;

// Lambda handler name: testjt
public class FunctionHandler implements RequestHandler<Map<String, Object>, Map<String, Object>> {
    private static final Logger LOG = LoggerFactory.getLogger(FunctionHandler.class);
    private final ApplicationContext ctx;
    private final AccountService accountService;

    public FunctionHandler() {
        // Initialize Micronaut context to get beans
        ctx = ApplicationContext.run();
        accountService = ctx.getBean(AccountService.class);
    }

    @Override
    public Map<String, Object> handleRequest(Map<String, Object> input, Context lambdaContext) {
        try {
            Account acc = accountService.getLastAccount();
            if (acc == null) {
                Map<String, Object> resp = new HashMap<>();
                resp.put("data", null);
                resp.put("message", "No account found");
                return resp;
            }
            Map<String, Object> resp = new HashMap<>();
            resp.put("data", acc.toMap());
            return resp;
        } catch (Exception e) {
            LOG.error("Error in Lambda handler while fetching account", e);
            Map<String, Object> err = new HashMap<>();
            Map<String, Object> details = new HashMap<>();
            details.put("message", e.getMessage());
            details.put("type", e.getClass().getName());
            err.put("error", details);
            return err;
        }
    }
}
