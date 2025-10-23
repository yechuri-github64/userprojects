"..."In "aws-lambda-tools-defaults.json", set "function-handler" to: "KailashcontractslambdaLambda::KailashcontractslambdaLambda.Function::kailashcontractslambda"

Sample Input (empty request):
{}

Sample Successful Output:
{
  "Id": 123,
  "ContractName": "Example Contract",
  "Email": "owner@example.com",
  "Error": null
}

Sample Error Output:
{
  "Id": null,
  "ContractName": null,
  "Email": null,
  "Error": {
    "Code": "DbError",
    "Message": "Detailed error message"
  }
}

Notes:
- The project targets .NET 8 and includes the Lambda serializer attribute.
- Connection string is loaded from appsettings.json ConnectionStrings.MySQL. The provided value is a dummy placeholder.
- Any exceptions are logged to stderr and returned in the Error field of the Response.
