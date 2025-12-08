Azure Functions (Node.js) - Salesforce Accounts Manager

This project provides Azure Functions to manage account records in Salesforce. It includes full CRUD and batch create operations. All functions use @azure/functions v4 API and jsforce for Salesforce connectivity.

Environment variables (local.settings.json):
- SF_LOGIN_URL
- SF_USERNAME
- SF_PASSWORD
- SF_TOKEN
- SF_CLIENT_ID
- SF_CLIENT_SECRET

Endpoints:
- POST /api/accounts -> createItem
 Input JSON: { "name": "Acme", "email": "a@acme.com", "address": "1 Main St" }
 Response: { success: true, data: { id: "<SF Id>" } }

- GET /api/accounts -> listItems
 Response: { success: true, data: [ { id, name, email, address }, ... ] }

- GET /api/accounts/{id} -> getItem
 Response: { success: true, data: { id, name, email, address } }

- PUT /api/accounts/{id} -> updateItem
 Input JSON: { "name": "New Name", "email": "new@acme.com", "address": "2 Main St" }
 Response: { success: true, data: { id } }

- DELETE /api/accounts/{id} -> deleteItem
 Response: { success: true, data: { id } }

- POST /api/accounts/batch -> batchCreate
 Input JSON: [ { "name": "A", "email": "a@x.com", "address": "Addr" }, { "name": "B" } ]
 Response: { success: true, data: [ { id, success, errors }, ... ] }

Notes:
- The Salesforce object used is the standard Account; custom fields Email__c and Address__c are used to store email and address.
- Errors are logged using console.log and returned in structured format: { success: false, error: { message } }.