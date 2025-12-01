Azure Functions Accounts API (Node.js Azure Functions v4)

Endpoints:
- POST /api/accounts -> createItem (body: { name, email, address })
- GET /api/accounts -> listItems
- GET /api/accounts/{id} -> getItem
- PUT/PATCH /api/accounts/{id} -> updateItem (body with any of name,email,address)
- DELETE /api/accounts/{id} -> deleteItem
- POST /api/accounts/batch -> batchCreate (body: [ { name, email, address }, ... ])

All responses: { success: boolean, data?: any, error?: { message: string, details?: any } }

Environment variables (local.settings.json): MYSQL_HOST, MYSQL_PORT, MYSQL_USER, MYSQL_PASSWORD, MYSQL_DATABASE

Database table (example):
CREATE TABLE accounts (
 id INT AUTO_INCREMENT PRIMARY KEY,
 name VARCHAR(255) NOT NULL,
 email VARCHAR(255) NOT NULL,
 address TEXT
);
