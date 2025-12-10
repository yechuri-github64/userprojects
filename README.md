# account-mangement API

This ASP.NET Core (net6.0) Web API provides basic account management against a MySQL backend using MySqlConnector. It follows a simple clean architecture with controllers, services, models, and a data access layer.

Running
- Configure database connection in appsettings.json under the Database section (Host, Port, Database, User, Password).
- Ensure the MySQL server has a database and a table named `accounts` with columns: id (INT AUTO_INCREMENT PRIMARY KEY), name (VARCHAR), email (VARCHAR), address (VARCHAR).

Example SQL to create table:

CREATE TABLE accounts (
 id INT AUTO_INCREMENT PRIMARY KEY,
 name VARCHAR(255),
 email VARCHAR(255),
 address VARCHAR(500)
);

- Build and run the project. The application listens on port 8080.

Endpoints
- GET /api/accounts - get all accounts
- GET /api/accounts/{id} - get a single account by id
- POST /api/accounts - create multiple accounts (accepts JSON array of accounts)
- PUT /api/accounts/{id} - update a single account (id in URL must match id in payload)

Notes
- The project uses MySqlConnector (v2.5.0) directly for DB access. Ensure your connection settings are correct.
- Logging and error handling are included. Adjust logging levels in appsettings.json as needed.
