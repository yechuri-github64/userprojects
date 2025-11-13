This project implements an AWS Lambda (Node.js) function to update a Salesforce Order record by Id.

Structure:
- index.js: Lambda entry point
- handlers/OrderHandler.js: Parses event, validates input, calls service
- services/SalesforceService.js: Handles Salesforce authentication and Order update using jsforce
- utils/response.js: Small helpers for HTTP responses
- .env: Environment variables required for Salesforce integration

Usage:
1. Populate the .env with your Salesforce credentials and settings.
2. Deploy the Lambda with node_modules (jsforce, dotenv) included.
3. Invoke the function with an event containing either:
   - pathParameters.orderId and a JSON body with fields to update
   - or body containing { "orderId": "<id>", ...fields }

Example event body:
{
  "orderId": "801xx0000000001AAA",
  "Status": "Activated",
  "EffectiveDate": "2025-11-13"
}

The function returns HTTP-style response objects with statusCode and JSON stringified body.
