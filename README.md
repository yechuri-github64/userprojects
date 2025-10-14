# accounts-management API

A minimal ASP.NET Core 8 Web API for managing accounts (id, name, email, address) using MySQL with Entity Framework Core. Supports bulk create (multiple accounts in one request) and single-record update.

Run
- Configure your MySQL credentials in appsettings.json -> ConnectionStrings:DefaultConnection
- Build and run:
  - dotnet restore
  - dotnet build
  - dotnet run
- The API listens on port 8080

Environment
- .NET 8
- MySQL (via Pomelo.EntityFrameworkCore.MySql)

Endpoints
- GET    /api/accounts             -> Get all accounts
- GET    /api/accounts/{id}        -> Get account by id
- POST   /api/accounts             -> Create one account
- POST   /api/accounts/bulk        -> Create multiple accounts at once
- PUT    /api/accounts/{id}        -> Update one account
- DELETE /api/accounts/{id}        -> Delete one account

Bulk Create Request Body (JSON)
[
  { "name": "Alice", "email": "alice@example.com", "address": "123 Main St" },
  { "name": "Bob", "email": "bob@example.com", "address": "456 Oak Ave" }
]

Single Create/Update Request Body (JSON)
{ "name": "Alice", "email": "alice@example.com", "address": "123 Main St" }

Notes
- The Email field is unique; duplicate emails will return HTTP 409 Conflict.
