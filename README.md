# RandomNumbersApi

A .NET 8 Web API demonstrating:
- A random number endpoint that returns two numbers between 30 and 90 as JSON
- A CRUD Items resource using Entity Framework Core (SQLite)
- Dependency injection, logging, and basic error handling

Prerequisites:
- .NET SDK 8.x

Getting started:
1. Restore and build
   - dotnet restore
   - dotnet build
2. Run
   - dotnet run
3. Open Swagger UI
   - https://localhost:5001/swagger or http://localhost:5000/swagger (ports may vary)

Endpoints:
- GET /api/random
  - Response: { "numbers": [n1, n2] }
- Items CRUD
  - GET /api/items
  - GET /api/items/{id}
  - POST /api/items  (body: { "name": "Example" })
  - PUT /api/items/{id} (body: { "name": "Updated" })
  - DELETE /api/items/{id}

Configuration:
- appsettings.json contains the SQLite connection string (DefaultConnection)

Notes:
- Database is created automatically on first run (EnsureCreated). For production, use EF Core migrations.