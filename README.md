test_project_main1

This ASP.NET Core Web API demonstrates a simple clean architecture for a single resource: Number.

Features:
- Controllers, Services, Models, and Data layers separated
- Entity Framework Core with SQLite for persistence (numbers.db)
- CRUD endpoints for Number resource
- Additional endpoint to retrieve numbers divisible by 5 and less than 500
- Basic error handling and console logging

Run:
1. dotnet restore
2. dotnet run

API endpoints:
- GET /api/numbers
- GET /api/numbers/divisible
- GET /api/numbers/{id}
- POST /api/numbers
- PUT /api/numbers/{id}
- DELETE /api/numbers/{id}

Note: The project focuses only on the minimal structure required to support the described functionality.
