# accounts-management API

- Tech stack: .NET 6, ASP.NET Core Web API, EF Core, MySQL (Pomelo provider)
- Port: 8080

Run steps:
1. Update appsettings.json Database section with your MySQL credentials.
2. Ensure the MySQL server has a database named "accountsdb" (or change the name in appsettings.json). The app will auto-create tables on first run.
3. dotnet restore
4. dotnet build
5. dotnet run --project accounts-management.csproj

HTTP Endpoints:
- GET    /api/accounts
- GET    /api/accounts/{id}
- POST   /api/accounts
  Body: { "name": "...", "email": "...", "address": "..." }
- POST   /api/accounts/batch
  Body: [ { "name": "...", "email": "...", "address": "..." }, ... ]
  Note: Allows creating multiple accounts in one request.
- PUT    /api/accounts/{id}
  Body: { "name": "...", "email": "...", "address": "..." }
  Note: Updates only one account at a time.
- DELETE /api/accounts/{id} 
