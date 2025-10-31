accounts-salesforce-app

This AWS Lambda provides CRUD operations for the Salesforce Account object using JSON over HTTP.

Environment variables required:
- SF_USERNAME - Salesforce username
- SF_PASSWORD - Salesforce password (append token in SF_TOKEN if separated)
- SF_TOKEN - Salesforce security token (optional if appended to password)
- SF_LOGIN_URL - optional, defaults to https://login.salesforce.com

Endpoints (HTTP method determines action):
- POST  -> create multiple accounts. Provide an array in the body or body.records.
  Example body: [{ "Name": "Acme", "Phone": "123" }, { "Name": "Beta" }]
  or { "records": [ { ... }, { ... } ] }

- GET   -> retrieve accounts. Provide query.id or body.id to get a single account. Otherwise can pass query.soql to run a SOQL query or will return a default list.
  Example: GET /?id=001xx000003DGbVAAW
  Example: GET /?soql=SELECT+Id,+Name+FROM+Account

- PATCH -> update a single account. Provide body.Id or body.id (or query.id) and fields to update.
  Example body: { "Id": "001xx000003DGbVAAW", "Phone": "555-1212" }

- DELETE -> delete a single account. Provide query.id or body.id.

Responses are JSON. Errors are returned in structured format: { "error": { "message": "...", "details": "...", "status": <code> } }

Logging: errors are logged to console.error for monitoring and diagnostics.
