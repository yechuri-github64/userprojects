<<<<<<< HEAD
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
=======
AWS Lambda Accounts Manager

This Lambda provides CRUD operations for an accounts table in a MySQL database. It follows a clean architecture with handlers and services.

Environment variables (set in Lambda or .env during local development):
- MySQL_HOST
- MySQL_USER
- MySQL_PASSWORD
- MySQL_DATABASE

API routes (API Gateway proxy expected):
- GET /accounts -> list all accounts
- GET /accounts/{id} -> get account by id
- POST /accounts -> create multiple accounts (body: JSON array of objects with name, email, address)
- PUT /accounts/{id} -> update one account (body: JSON object with any of name, email, address)
- DELETE /accounts/{id} -> delete account

Notes:
- Uses mysql2/promise
- Input body is parsed with JSON.parse and responses use JSON.stringify
- Create accepts multiple accounts in one request and uses a multi-row insert
- Update modifies one account at a time

Deploy:
1. Install dependencies: npm install
2. Package and deploy the Lambda with your preferred method, ensure environment variables are set

Database table example:
CREATE TABLE accounts (
  id INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(255),
  email VARCHAR(255),
  address TEXT
);
>>>>>>> bd324d8 (Automated commit on branch accounts-management-lambda from AI2DEV)
