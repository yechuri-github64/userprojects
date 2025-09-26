lucky-number API

This ASP.NET Core (.NET 8) Web API exposes an endpoint to generate a random number between 5 and 500 and return it as JSON without persisting it. The project follows simple clean architecture separation (Controllers, Services, Models, Data) and uses EF Core InMemory provider to support CRUD operations on the GeneratedNumber resource.

Endpoints:
- GET /api/GeneratedNumbers/generate?min=5&max=500  -> returns { "value": number }
- CRUD endpoints under /api/GeneratedNumbers for managing persisted GeneratedNumber records (optional):
  - GET /api/GeneratedNumbers
  - GET /api/GeneratedNumbers/{id}
  - POST /api/GeneratedNumbers
  - PUT /api/GeneratedNumbers/{id}
  - DELETE /api/GeneratedNumbers/{id}

Notes:
- The generate endpoint does not store the generated number in the database.
- The project uses an in-memory EF Core database; no external database is required.

To run:
1. dotnet restore
2. dotnet run

The API will be available at the configured URL (e.g., https://localhost:5001).