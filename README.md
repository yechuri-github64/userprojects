Node.js AWS Lambda for Salesforce Accounts CRUD

Structure:
- index.js: Lambda entry
- handlers/AccountsHandler.js: Routes requests to service
- services/SalesforceService.js: Salesforce connection and CRUD logic (uses jsforce)
- utils/response.js: Simple response helpers
- .env: Environment variables for Salesforce (see file)

Environment variables (place in .env or Lambda environment):
SALESFORCE_CLIENT_ID
SALESFORCE_CLIENT_SECRET
SALESFORCE_USERNAME
SALESFORCE_PASSWORD
SALESFORCE_SECURITY_TOKEN
SALESFORCE_USE_SANDBOX
SALESFORCE_INSTANCE_URL
SALESFORCE_LOGIN_URL
SALESFORCE_API_VERSION
SALESFORCE_TIMEOUT_SECONDS
SALESFORCE_ENABLE_LOGGING
SALESFORCE_MAX_RETRIES
SALESFORCE_RETRY_DELAY

Endpoints (API Gateway proxy expected):
- POST /accounts -> create account (JSON body with Account fields)
- GET /accounts/{id} -> get account by Id
- GET /accounts -> list accounts (optional query param q for SOQL)
- PUT /accounts/{id} -> update account (JSON body with fields)
- DELETE /accounts/{id} -> delete account

Notes:
- Uses jsforce@3.10.7
- Service uses username+password+security token login flow
- Uses async/await and basic retry logic for connection
