This Lambda fetches a Salesforce Order by Id using the standard Order object.

Structure:
- index.js: Lambda entry that delegates to handlers.
- handlers/OrderHandler.js: Parses input and returns formatted responses.
- services/SalesforceService.js: Connects to Salesforce and retrieves Order data.
- utils/response.js: Builds HTTP responses.

Environment variables (.env):
Populate the .env file with Salesforce credentials and settings as provided.

Invoke:
Provide an event with orderId in one of:
- pathParameters.orderId
- queryStringParameters.orderId
- body (JSON) { "orderId": "..." }

Response:
Returns JSON representation of the Order with common fields.

Logging:
Uses console.log for diagnostics.

Dependencies:
- jsforce
- dotenv

Security:
Do not commit credentials. Use AWS Secrets Manager or encrypted environment in production.
