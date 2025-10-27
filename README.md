# test-acc-sf-app

This ASP.NET Core (.NET 8) Web API provides CRUD operations against the Salesforce Accounts object.

- Application port: 8080 (configured via appsettings.json)
- Data provider: Salesforce (configured via appsettings.json)

Configuration

- Edit appsettings.json and set the Salesforce.InstanceUrl, ClientId, ClientSecret, Username and Password (password+security token if applicable).

Run

- dotnet restore
- dotnet run

Endpoints

- POST /api/accounts -> Create multiple accounts with a JSON array body
- GET /api/accounts/{id} -> Retrieve a single account
- PUT /api/accounts/{id} -> Update a single account (one at a time)
- DELETE /api/accounts/{id} -> Delete a single account

Notes

- The project uses a Salesforce client that authenticates using the OAuth2 password grant to obtain an access token and then calls Salesforce REST endpoints.
- No local database is used.
