Project: accountslamda

Description:
This AWS Lambda (.NET 8) function returns the last created account for a provided id from the "accounts" table in a MySQL database. The function handler is named "accountslamda" and the assembly name is AccountslamdaLambda.

Configuration:
- Edit appsettings.json to set the proper MySQL connection string under ConnectionStrings:MySQL.
- The function handler is configured in aws-lambda-tools-defaults.json as:
  "AccountslamdaLambda::AccountslamdaLambda.Function::accountslamda"

Sample input JSON:
{
  "Id": 123
}

Sample successful output JSON:
{
  "Success": true,
  "Data": {
    "id": 123,
    "name": "Example",
    "email": "example@example.com",
    "created_at": "2025-01-01 12:00:00"
  },
  "Error": null
}

Sample error output JSON (structured):
{
  "Success": false,
  "Data": null,
  "Error": {
    "Code": "ServerError",
    "Message": "...",
    "Details": "..."
  }
}

Notes:
- The service reads appsettings.json at runtime to get the connection string named "MySQL".
- Any database or runtime error is logged via the Lambda context logger and returned in the structured Error object in the Response.
