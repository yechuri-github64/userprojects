AWS Lambda: Fetch Salesforce Account by accountId

Overview:
- This Lambda retrieves a Salesforce Account record by accountId passed in the function URL.
- It removes fields where the value is null or empty and returns the cleaned account JSON.

Structure:
- index.js: Lambda entry point that delegates to handlers/AccountHandler.js
- handlers/AccountHandler.js: Parses APIGatewayProxyEvent and implements minimal CRUD scaffolding. GET calls SalesforceService.
- services/SalesforceService.js: Calls Salesforce REST API and contains a cleanObject utility to remove null/empty fields.

Environment variables (required):
- SF_INSTANCE_URL: e.g. https://yourDomain.my.salesforce.com
- SF_ACCESS_TOKEN: OAuth access token (Bearer)
- SF_API_VERSION: optional, default v57.0

Usage (GET):
- Provide accountId via path parameter named accountid or accountId, or as query string parameter.
- Example function URL: https://.../Prod/account/001xx000003DGb9AAG

Responses:
- 200: cleaned account JSON
- 400: missing accountId
- 404: account not found
- 500: internal/server errors

Notes:
- Add appropriate IAM permissions and secure storing of Salesforce access token.
- This project uses node-fetch v2 for HTTP calls.
