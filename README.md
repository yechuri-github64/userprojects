# accounts - Local dev

1. npm install
2. Update local.settings.json with local DB credentials
3. Ensure you have an `accounts` table in your MySQL database with schema similar to:
 CREATE TABLE accounts (
 id INT AUTO_INCREMENT PRIMARY KEY,
 name VARCHAR(255) NOT NULL,
 email VARCHAR(255) NOT NULL,
 address TEXT
 );
4. npm run start:functions to start Azure Functions locally (Azure Functions Core Tools required)
5. Use Postman/Thunder Client to hit endpoints:
 - POST /items -> create single account (body: { name, email, address })
 - POST /items/batch -> create multiple accounts (body: [ {name,email,address}, ... ])
 - GET /items -> list all accounts
 - GET /items/{id} -> get single account
 - PUT /items/{id} -> update single account (partial allowed)
 - DELETE /items/{id} -> delete account

All responses are JSON: { success: boolean, data: any, error?: string }
