This AWS Lambda function retrieves a Salesforce Order by Id using the standard Order object.

Structure:
- index.js: Lambda entrypoint
- handlers/OrderHandler.js: Lambda handler (parses input, returns HTTP response)
- services/SalesforceService.js: Salesforce connection and query logic (uses jsforce)
- utils/utils.js: helper to remove empty fields from the response
- .env: environment variables for Salesforce credentials and settings

Usage:
1. Populate .env with your Salesforce credentials and settings.
2. Deploy as an AWS Lambda with Node.js runtime.
3. Invoke with an event containing pathParameters.orderId or a JSON body with { "orderId": "..." }.

Responses:
- 200: JSON of the Order with empty fields removed
- 400: Missing orderId
- 404: Order not found
- 500: Internal server error

Notes:
- Uses jsforce for Salesforce integration.
- Retries Salesforce login based on SALESFORCE_MAX_RETRIES and SALESFORCE_RETRY_DELAY.
