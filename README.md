# accounts-salesforce-app

This AWS Lambda project provides a single handler named `accounts-salesforce-app` to create, retrieve, update and delete Salesforce Account records.

Downstream: Salesforce
Conn Name: Salesforce

Environment variables required:
- SF_USERNAME (Salesforce username)
- SF_PASSWORD (Salesforce password)
- SF_TOKEN (optional: Salesforce security token)
- SF_LOGIN_URL (optional: https://login.salesforce.com or https://test.salesforce.com)

HTTP behavior (expects JSON input and returns JSON output):
- POST / -> Create multiple accounts
  - Body: { "accounts": [ { "Name": "Acme" , ... }, ... ] }
  - Response: { created: [...], errors: [...] }

- GET /?id=001... -> Retrieve single account by id
  - Response: { account: { ... } }

- GET /?soql=SELECT+Id+FROM+Account -> Run a SOQL query
  - Response: { totalSize, done, records }

- GET / -> Default list: returns up to 200 accounts
  - Response: { totalSize, records }

- PATCH / or PUT / -> Update one account at a time
  - Body: { "id": "001...", "fields": { "Name": "New Name" } }
  - Response: { updated: { id: "001..." } }

- DELETE /?id=001... or DELETE / with body { "id": "001..." } or { "ids": ["001...", ...] }
  - Response: { deleted: [...], errors: [...] }

All errors are returned in a structured JSON format: { "error": { "message": "...", "code": "...", "details": ... } }

Files included:
- index.js (Lambda handler exports['accounts-salesforce-app'])
- package.json
- delete.json, get.json, patch.json, post.json (as provided)

Install dependencies with `npm install` before packaging for Lambda.
