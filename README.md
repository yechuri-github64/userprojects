# Accounts Management C# API

This project is an ASP.NET Core (.NET 8.0) Web API implementing account management with support for multiple backends: Salesforce, MySQL, and PostgreSQL. It follows a clean architecture with controllers, services, and a data access layer.

Key points:
- API listens on port 8080 (Kestrel configured)
- Supports creating multiple accounts at once (POST /api/accounts)
- Updating a single account at a time (PUT /api/accounts/{id})
- Uses appsettings.json to configure backend provider and connection settings
- Salesforce authentication is implemented using OAuth2 username/password flow and requires configuration in appsettings.json
- For Salesforce operations the API uses the configured instanceUrl returned by Salesforce after authentication

Important configuration (appsettings.json):
- Backend:Provider -> set to "mysql", "postgresql" or "salesforce"
- MySql.* and Postgres.* sections for database connectivity
- Salesforce.* contains LoginUrlProduction, LoginUrlSandbox, UseSandbox, ClientId, ClientSecret, Username, Password, SecurityToken

Packages included in project file:
- MySqlConnector 2.5.0
- Npgsql 7.0.5

Notes about jsforce:
- The original requirement referenced jsforce@3.10.7 (a Node.js Salesforce library). If you need to use jsforce in conjunction with this project, run a separate Node.js service or script and install jsforce with: npm install jsforce@3.10.7

Error handling and logging are present throughout services and controllers. The application deliberately avoids using EF Core to allow direct use of MySqlConnector and Npgsql as requested.

To run:
1. Update appsettings.json with appropriate connection and Salesforce credentials.
2. dotnet restore
3. dotnet build
4. dotnet run

The API endpoints:
- GET /api/accounts
- GET /api/accounts/{id}
- POST /api/accounts (accepts JSON array of accounts)
- PUT /api/accounts/{id}
- DELETE /api/accounts/{id}
- GET /api/accounts/salesforce/{salesforceId} (direct Salesforce object GET)
