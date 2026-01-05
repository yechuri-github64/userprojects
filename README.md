Demotestproj Lambda

This project implements an AWS Lambda function (C# .NET 8) to create travelcards and associated cardholders. It validates input according to the specification, persists to a PostgreSQL backend, and returns a travelcardId and token.

Configuration
- Set the PostgreSQL connection string in appsettings.json under ConnectionStrings:PostgreSql or via environment variable "PostgreSql".

Build & Deploy
- Use dotnet tooling and AWS Lambda tools with configured aws-lambda-tools-defaults.json.
