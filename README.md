This AWS Lambda handles retrieving and updating orders in Salesforce.

Structure:
- index.js: Lambda entry that delegates to handlers/OrdersHandler.js
- handlers/OrdersHandler.js: Parses the event and supports GET (retrieve) and PUT (update) for orders.
- services/SalesforceService.js: Connects to Salesforce using jsforce and provides getOrder and updateOrder.
- services/utils.js: Small JSON helpers.

Environment:
Provide Salesforce credentials and settings in the .env file. dotenv is loaded in the Salesforce service.

Behavior:
- GET /orders/{orderId} -> returns Salesforce Order record
- PUT /orders/{orderId} with JSON body -> updates Salesforce Order (fields in body)

Notes:
- Uses async/await, JSON.parse/stringify for input/output, and console.log for logging.
- Minimal, essential code only.