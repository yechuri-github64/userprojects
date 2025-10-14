# accounts-management API

Overview:
- ASP.NET Core 8 Web API following clean architecture separation: Controllers, Services, Models, Data.
- MySQL backend using Entity Framework Core (Pomelo provider).
- Supports create (single and bulk), read, update (single only), and delete operations for accounts with fields: id, name, email, address.
- Configured to listen on port 8080.

Run steps:
1) Ensure MySQL is available and create a database (default: accountsdb):
   - Update appsettings.json with proper Server, Port, Database, User, Password.
2) Restore and run:
   - dotnet restore
   - dotnet build
   - dotnet run
3) API base URL: http://localhost:8080

Endpoints:
- GET    /api/accounts
- GET    /api/accounts/{id}
- POST   /api/accounts            (create single)
- POST   /api/accounts/bulk       (create multiple in one request)
- PUT    /api/accounts/{id}       (update single)
- DELETE /api/accounts/{id}

Notes:
- Uses appsettings.json for DB provider, connection details, and Kestrel port configuration.
- Add EF Core migrations if desired:
  - dotnet ef migrations add Init --project .
  - dotnet ef database update
