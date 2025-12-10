AWS Lambda Accounts Manager

Environment
- Provide MySQL connection via .env (see .env file). The connection name is MySQL (MYSQL_CONN_NAME=MySQL).

Usage
- Deploy as a single Lambda handler (index.handler).
- HTTP methods map to operations:
  - GET /accounts -> returns all accounts
  - GET /accounts/{id} -> returns account by id
  - POST /accounts -> create multiple accounts (request body: JSON array of { name, email, address })
  - PUT /accounts/{id} -> update a single account (request body: JSON object with any of name, email, address)
  - DELETE /accounts/{id} -> delete a single account

Notes
- Uses mysql2 with async/await and a connection pool.
- Handlers, services, and utils follow a clean architecture separation.
- All responses are JSON-stringified. Logs are written with console.log.