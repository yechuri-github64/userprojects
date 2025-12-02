Azure Functions for Salesforce Accounts

This project provides Azure Functions (Node.js, Functions V4) to perform CRUD operations on Salesforce Account records, including batch creation. Each function is implemented using the @azure/functions v4 API and uses jsforce to interact with Salesforce.

Environment Variables (set in local.settings.json or in your App Settings):
- SF_LOGIN_URL (e.g. https://login.salesforce.com)
- SF_USERNAME
- SF_PASSWORD
- SF_TOKEN
- SF_CLIENT_ID
- SF_CLIENT_SECRET

Functions:
- POST /api/accounts -> createItem (create a single Account)
- GET /api/accounts -> listItems (list Accounts)
- GET /api/accounts/{id} -> getItem (retrieve a single Account)
- PUT /api/accounts/{id} -> updateItem (update one Account by Id)
- DELETE /api/accounts/{id} -> deleteItem (delete one Account by Id)
- POST /api/accounts/batch -> batchCreate (create multiple Accounts with a single JSON array)

Input/Output:
- All functions accept and return JSON.
- Each response has shape: { success: boolean, data?: any, error?: { message: string, details?: any } }
- Functions return { status: <code>, jsonBody: {...} } compatible with the app model.

Notes:
- Uses jsforce to connect to Salesforce via username/password+token. The connection helper is in src/helpers/salesforce/connection.js.
- Logs are written with console.log().