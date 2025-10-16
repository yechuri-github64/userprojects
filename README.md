# accounts-sf-sa

- Prerequisites: .NET 8 SDK, Salesforce Connected App with OAuth Username-Password flow enabled.
- Configure appsettings.json under the Salesforce section with ClientId, ClientSecret, Username, Password, and SecurityToken. Optionally set UseSandbox, TokenUrl, and InstanceUrl.
- Run: dotnet restore && dotnet run --urls http://0.0.0.0:8080

Endpoints (JSON in/out):
- POST /api/accounts
  - Body can be an array of account objects or { "accounts": [ ... ] }
  - Creates multiple accounts in Salesforce and returns per-item results.
- GET /api/accounts/{id}
  - Retrieves a Salesforce Account by Id.
- PATCH /api/accounts/{id}
  - Body: JSON object with fields to update.
- DELETE /api/accounts/{id}
  - Deletes the specified Account.

Notes:
- Uses EF Core InMemory for request logging and ASP.NET Core DI.
- Service authenticates to Salesforce via OAuth 2.0 Username-Password flow and calls REST API.
