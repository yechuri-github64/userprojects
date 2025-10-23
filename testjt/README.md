# testjt

Micronaut Java AWS Lambda project (Java 21)

Structure:
- Application entry: Testjt.Application
- Lambda handler: Testjt.FunctionHandler (custom property lambda.handler: testjt)
- Controller: Testjt.controllers.AccountController
- Service: Testjt.AccountService
- Model: Testjt.models.Account

Database:
- Downstream: MySQL
- Connection name: MySQL
- Configure JDBC URL, username, password in src/main/resources/application.yml

Sample usage (Lambda invocation):

Input: (any JSON, no specific payload required)
{}

Successful output (example):
{
  "data": {
    "id": 42,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "createdAt": "2025-10-01 12:34:56"
  }
}

If no record exists:
{
  "data": null,
  "message": "No account found"
}

If an error occurs (structured error):
{
  "error": {
    "message": "Detailed error message",
    "type": "java.lang.RuntimeException"
  }
}

Notes:
- Replace database placeholders in application.yml before deploying.
- The service queries: SELECT id, name, email, created_at FROM accounts ORDER BY id DESC LIMIT 1
- Any exception is logged and returned as structured JSON under the "error" key.
