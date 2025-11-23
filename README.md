This project is a minimal ASP.NET Core (.NET 8) Web API that provides CRUD operations against Salesforce Account objects.

Key points:
- Namespace used in code: demo_test_accounts_salesforce_app
- Configured via appsettings.json (SalesforceSettings) including ProductionLoginUrl, SandboxLoginUrl, UseSandbox, ClientId, ClientSecret, Username, Password, SecurityToken, InstanceUrl, ApiVersion, Port and Provider.
- The API listens on port 8080 by default (configurable in appsettings.json).
- Endpoints:
 - GET /api/accounts/{id}
 - POST /api/accounts
 - PUT /api/accounts/{id}
 - DELETE /api/accounts/{id}

Notes:
- The service authenticates using the OAuth2 password grant to retrieve an access token and instance URL, then uses the Salesforce REST API to perform operations on the Account object.
- No local database is used.
- Ensure you populate the SalesforceSettings in appsettings.json with valid credentials and client information before running.
