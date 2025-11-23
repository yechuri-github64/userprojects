This AWS Lambda updates a Salesforce Order record using the Salesforce REST API via jsforce.

Usage
- Populate the .env file with your Salesforce credentials and settings.
- Deploy the Lambda with NODE environment and install dependencies (jsforce).

Input
- The Lambda expects event.body to be a JSON string (Salesforce standard object JSON) containing either:
  - Id: the Order Id, or
  - orderId: the Order Id
- Other fields in the JSON are treated as Order fields to update.

Example input body:
{ "Id": "801xx0000001gPDAAY", "Status": "Activated", "EffectiveDate": "2025-11-23" }

Behavior
- The handler parses the input, removes the Id field from the payload, performs an update against the Order sObject, and returns the full Order record as retrieved from Salesforce.
- Retries are performed on login based on SALESFORCE_MAX_RETRIES and SALESFORCE_RETRY_DELAY.

Project structure
- index.js: Lambda entry
- handlers/OrderHandler.js: input validation and orchestration
- services/SalesforceService.js: Salesforce connection, login, update, retrieve
- utils/retry.js: retry helper
- utils/response.js: JSON parse and HTTP-style responses

Notes
- Uses jsforce package for Salesforce connectivity.
- Console logs are used for tracing and errors.
