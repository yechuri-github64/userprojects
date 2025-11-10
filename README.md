Project: Accounts Lambda (MySQL downstream)

Overview:
- AWS Lambda (Node.js) to manage accounts table in a MySQL database.
- Supports: get all accounts, get single account, create multiple accounts at once (POST array), update exactly one account (PUT), delete one account (DELETE).
- Clean architecture with handlers and services. Uses async/await and proper error handling.

Files:
- index.js: Lambda entry point.
- handlers/AccountsHandler.js: Parses HTTP method and delegates to service.
- services/AccountsService.js: Core DB logic (get, create multiple, update one, delete).
- services/db.js: MySQL pool helper using mysql2/promise.
- .env: Environment variables template.

Environment (set these before deployment):
- DB_HOST, DB_USER, DB_PASSWORD, DB_NAME, DB_PORT
- CONN_NAME=MySQL

API (assumes API Gateway proxy integration):
- GET /accounts -> returns all accounts
- GET /accounts/{id} -> returns single account
- POST /accounts -> body: JSON array of account objects [{"name":"","email":"","address":""}, ...]
- PUT /accounts/{id} -> body: JSON object with updatable fields (name, email, address)
- DELETE /accounts/{id} -> deletes specified account

Notes:
- Input and output bodies use JSON.parse / JSON.stringify.
- create (POST) inserts multiple records in a single transaction.
- update (PUT) updates one account at a time; it requires an id and at least one updatable field.
- Logging is done via console.log.

Database:
- Table expected: accounts with columns id (primary key, auto-increment), name, email, address.

Install:
- npm install

Deploy:
- Package and deploy to AWS Lambda; ensure environment variables are configured to connect to your MySQL instance.