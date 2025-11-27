Azure Functions (Node.js) - Salesforce Accounts

This project provides Azure Functions to perform CRUD operations and batch creation for Account records in Salesforce. It uses jsforce for Salesforce connectivity and the @azure/functions v4 programming model.

Environment variables (set in local.settings.json or your Function App settings):
- SF_LOGIN_URL
- SF_USERNAME
- SF_PASSWORD
- SF_TOKEN
- SF_CLIENT_ID
- SF_CLIENT_SECRET

Endpoints:
- POST /api/accounts -> createItem (body: { name, email, address })
- GET /api/accounts -> listItems
- GET /api/accounts/{id} -> getItem
- PUT /api/accounts/{id} -> updateItem (body: { name?, email?, address? })
- DELETE /api/accounts/{id} -> deleteItem
- POST /api/accounts/batch -> batchCreate (body: [{ name, email, address }, ...])

Responses are JSON with shape: { success: boolean, data?: any, error?: { message, details? } }

Logging: Uses console.log for error/info logs.

Setup:
1. npm install
2. Configure environment variables
3. func start
