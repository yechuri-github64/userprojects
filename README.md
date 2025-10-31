AWS Lambda: Salesforce Account Fetcher

Overview:
This Lambda implements a small CRUD handler for Salesforce Account records and returns Account JSON with null/empty fields removed.

Entry point:
- index.js -> exports.handler
- handlers/AccountHandler.js -> parses APIGateway event and routes methods (GET, POST, PUT/PATCH, DELETE)
- services/SalesforceService.js -> performs REST calls to Salesforce using instance URL and access token
- utils/cleaner.js -> removes null/empty fields recursively

Environment variables required:
- SF_INSTANCE_URL : e.g. https://yourInstance.my.salesforce.com
- SF_ACCESS_TOKEN : OAuth access token with appropriate API permissions
- SF_API_VERSION  : (optional) e.g. 56.0 (defaults to 56.0)

Usage:
- GET /{accountid} -> returns Account JSON (with null/empty fields removed)
- POST -> create account (pass JSON body)
- PUT/PATCH /{accountid} -> update account (pass JSON body)
- DELETE /{accountid} -> delete account

Notes:
- The function expects API Gateway proxy integration; path parameter name used is 'accountid' (case-insensitive checks included).
- Responses are application/json. Errors include a message and appropriate HTTP status codes.

Install:
- npm install

Deploy:
- Zip and upload to Lambda or use your deployment pipeline. Ensure environment variables are set in Lambda configuration.
