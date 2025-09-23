# Order Management Lambda

This AWS Lambda (Node.js) project accepts order payloads and saves them into a MySQL `orders` table.

Files included:
- index.js - main Lambda handler (exports.ordermanagement)
- config/env.js - loads environment variables from .env using dotenv
- config/db.js - MySQL connection pool using mysql2/promise
- utils/helper.js - validation and error formatting utilities
- tests/index.test.js - Jest unit tests
- .env - sample environment variables

Environment
-----------
Place your DB connection details into the .env file or provide them using Lambda environment variables. Sample .env provided in this repo.

Sample .env content (also provided as orders_connMySQL.yaml):

DATABASE=MySQL
DATABASE_HOST=localhost
DATABASE_USER=""
DATABASE_PASSWORD=""
DATABASE_NAME=orders_conn

Deploy to AWS Lambda
--------------------
- Create a Lambda function and upload this project bundle (node_modules required) or build a ZIP including dependencies.
- Set handler to index.ordermanagement
- Ensure VPC and security groups allow the Lambda function to reach the MySQL host.

Input JSON (body)
-----------------
{
  "customername": "Alice",
  "id": "ord-100",
  "order_amount": 49.99,
  "quantity": 2,
  "subscription": true
}

Success Response
----------------
StatusCode: 201
Body:
{
  "message": "Order saved",
  "data": { "insertedId": 123, "customername": "Alice", "id": "ord-100", "order_amount": 49.99, "quantity": 2, "subscription": true }
}

Error Response (validation)
---------------------------
StatusCode: 400
Body:
{
  "error": {
    "message": "Validation failed",
    "details": [ { "field": "...", "message": "..." } ]
  }
}

Error Response (server/db)
--------------------------
StatusCode: 500
Body:
{
  "error": {
    "message": "...",
    "code": "...",
    "details": "..."
  }
}

Testing
-------
Install dev dependencies and run tests:

npm install
npm test

