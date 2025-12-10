# account-c-sharp API

This project is an ASP.NET Core (.NET 8) Web API for managing accounts stored in a MySQL database. It follows clean architecture principles with separation between controllers, services, and data access.

Usage:
- Configure MySQL connection in appsettings.json under the Connection section (Host, Port, Database, User, Password).
- The application listens on port 8080 by default (AppSettings:Port).

Endpoints:
- GET /api/accounts -> get all accounts
- GET /api/accounts/{id} -> get account by id
- POST /api/accounts -> create multiple accounts (accepts JSON array)
- PUT /api/accounts/{id} -> update a single account (id in route must match id in body)
- DELETE /api/accounts/{id} -> delete a single account

Notes:
- Uses MySqlConnector for database access.
- The application will ensure the accounts table exists on startup.
- Proper logging and error handling are included.
