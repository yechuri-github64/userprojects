Overview

This AWS Lambda project creates an Order record in Salesforce using the Salesforce REST API.

Structure

- index.js - Lambda entry that delegates to the handler
- handlers/OrderHandler.js - input validation and orchestration
- services/SalesforceService.js - authenticates with Salesforce and creates Order records
- services/utils.js - simple retry helper
- .env - environment variables for Salesforce credentials and configuration

Usage

1) Populate .env with your Salesforce credentials and settings.
2) Deploy this code to AWS Lambda (Node 14+).
3) Invoke the Lambda with a JSON payload containing the order fields. Example body:

{ "order": { "AccountId": "001...", "EffectiveDate": "2025-11-12", "Status": "Draft" } }

The Lambda returns the Salesforce create response (id/success/errors) inside the response body.

Notes

- Uses OAuth2 resource owner password credentials to obtain an access token (salesforce password + security token).
- Retries and timeouts are configurable via .env.
