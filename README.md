Test Project Lambda - test_project_main

This AWS Lambda function returns a random integer between a provided min and max (inclusive) and responds with JSON.

Endpoint behavior (GET request expected, no body):
- Query parameters:
  - min (optional, integer, default 200)
  - max (optional, integer, default 500)

Response (200):
{
  "number": 347
}

Errors are returned in a structured JSON format, logged via Lambda logs.

Sample request (API Gateway):
GET /?min=200&max=500

Environment and backend connector:
- ConnectorHelper will attempt to use a MySQL connection if a connection string is supplied via the environment variable MYSQL_CONNECTION or by reading a get.json config file placed alongside the function package.

Build and deploy:
- dotnet build
- Use AWS Toolkit / dotnet lambda deploy-serverless or the AWS CLI with the provided aws-lambda-tools-defaults.json settings.
