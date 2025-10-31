AWS Lambda to perform CRUD operations on Salesforce Account object

Usage

- Provide SF_INSTANCE_URL (e.g. https://yourInstance.salesforce.com) and SF_ACCESS_TOKEN as environment variables.
- Optionally set SF_API_VERSION (defaults to v58.0).

Endpoints (via API Gateway mapping to this Lambda)

- POST /accounts
  - Body: Salesforce Account JSON (e.g. { "Name": "Acme" })
  - Response: created Account object

- GET /accounts/{id}
  - Response: Account object

- PUT /accounts/{id}
  - Body: fields to update
  - Response: updated Account object

- DELETE /accounts/{id}
  - Response: 204 No Content

Notes

- Uses axios to call Salesforce REST API under /services/data/{version}/
- Expects a valid Salesforce OAuth access token in SF_ACCESS_TOKEN
- Uses async/await and returns JSON responses
