# accounts-management API

Overview:
- ASP.NET Core Web API (net8.0) following a clean architecture separation: Controllers, Services, Models, Data
- MySQL backend via EF Core (Pomelo provider)
- Bulk create (multiple accounts at once), single-account update, and standard CRUD
- JSON payloads use camelCase: id, name, email, address
- Listens on port 8080

Endpoints:
- GET /api/accounts
- GET /api/accounts/{id}
- POST /api/accounts/bulk
- PUT /api/accounts/{id}
- DELETE /api/accounts/{id}

Setup:
1) Update appsettings.json ConnectionStrings:DefaultConnection with your MySQL credentials.
2) Ensure MySQL is running and the database (accountsdb) exists.
3) Run migrations if you add them in the future; current model will be created by EF if using EnsureCreated/Manual migrations.
4) Run the app:
   - dotnet restore
   - dotnet run

Notes:
- The API is configured to listen on http://0.0.0.0:8080.
- Logging is enabled to Console and Debug.
- Email field is unique; bulk create will fail for duplicates.