AWS Lambda to manage Salesforce Account records

Files:
- index.js: Lambda export
- handlers/AccountHandler.js: API Gateway handler implementing CRUD (POST, GET, PUT/PATCH, DELETE)
- services/SalesforceService.js: Salesforce authentication and REST operations
- services/utils.js: small helpers

Environment variables (provided in .env):
SALESFORCE_CLIENT_ID, SALESFORCE_CLIENT_SECRET, SALESFORCE_USERNAME, SALESFORCE_PASSWORD, SALESFORCE_SECURITY_TOKEN, SALESFORCE_USE_SANDBOX, SALESFORCE_INSTANCE_URL, SALESFORCE_LOGIN_URL, SALESFORCE_API_VERSION, SALESFORCE_TIMEOUT_SECONDS, SALESFORCE_ENABLE_LOGGING, SALESFORCE_MAX_RETRIES, SALESFORCE_RETRY_DELAY

Deploy as a Node.js Lambda. The handler supports API Gateway proxy events. POST creates an Account (returns full account JSON). GET/PUT/DELETE operate on /{id}.
