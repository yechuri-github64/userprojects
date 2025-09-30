# accounts-operation API

This project is a minimal ASP.NET Core (net8.0) Web API that provides CRUD operations for an "accounts" table in an SQLite database named orders_management.

Project structure follows a simple clean architecture separation: Controllers, Services, Models, and Data (EF Core DbContext).

How to run:

1. Ensure .NET 8 SDK is installed.
2. dotnet restore
3. dotnet build
4. dotnet run

The API will create the SQLite database file (orders_management.db) automatically on first run. Endpoints are available under /api/accounts.
