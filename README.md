This project implements a minimal ASP.NET Core (.NET 8) Web API for managing Salesforce Account records via a backend Salesforce REST connection.

Features:
- Add multiple accounts in a single JSON POST to /api/accounts
- Retrieve all accounts: GET /api/accounts
- Retrieve single account: GET /api/accounts/{id}
- Update a single account at a time: PUT /api/accounts/{id}
- Delete an account: DELETE /api/accounts/{id}

Structure follows a simple Clean Architecture separation: Controllers, Services, Models, Data.

Configuration:
- appsettings.json contains Application port (8080), Database provider, ConnectionStrings and Salesforce connectivity settings (Host, Port, ApiVersion, AuthToken, etc.).
- Program.cs configures the DbContext (InMemory by default) and an HttpClient for Salesforce.

Notes:
- Provide Salesforce authentication token in appsettings.json at Salesforce:AuthToken or configure authentication in Program.cs before running.
- The code uses an InMemory EF provider by default for simplicity. Change Database:Provider to "SqlServer" and set ConnectionStrings:DefaultConnection to use SQL Server.

To run:
- dotnet build
- dotnet run

The application listens on port 8080 by default.
