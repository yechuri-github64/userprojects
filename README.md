AWS Lambda to retrieve Order(s) from Salesforce

Structure:
- index.js: Lambda entry.
- handlers/OrdersHandler.js: parses input, routes requests.
- services/SalesforceService.js: handles Salesforce auth and queries.

Usage:
- Provide Salesforce credentials in .env (see .env file included).
- Invoke Lambda with optional orderId via pathParameters.orderId or queryStringParameters.orderId or JSON body { "orderId": "..." }.
- Responses are JSON with shape { success: boolean, data: ... } or error message.

Notes:
- Uses username-password OAuth2 flow. Ensure security token is appended to password if required.
- Uses axios and dotenv. Install dependencies before deployment.
