using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using TestfromdocspostgresqlLambda.Models;
using TestfromdocspostgresqlLambda.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TestfromdocspostgresqlLambda
{
    public class Function
    {
        private readonly Service _service;
        private readonly JsonSerializerOptions _jsonOptions;

        public Function()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
            var configuration = builder.Build();
            _service = new Service(configuration);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            // Add converters for enum exact case
            _jsonOptions.Converters.Add(new Models.ExactCaseEnumConverter<RailcardType>());
            _jsonOptions.Converters.Add(new Models.ExactCaseEnumConverter<CardholderType>());
        }

        // Lambda handler must match the name used in aws-lambda-tools-defaults.json
        public async Task<APIGatewayProxyResponse> testfromdocspostgresql(APIGatewayProxyRequest request, ILambdaContext context)
        {
            Console.WriteLine("Lambda invoked: testfromdocspostgresql");
            try
            {
                // Extract headers as required
                request.Headers ??= new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                request.QueryStringParameters ??= new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                var clientId = request.Headers.ContainsKey("client_id") ? request.Headers["client_id"] : null;
                var userAgent = request.Headers.ContainsKey("User-Agent") ? request.Headers["User-Agent"] : null;
                var contentType = request.Headers.ContainsKey("Content-Type") ? request.Headers["Content-Type"] : null;
                var correlation = request.Headers.ContainsKey("X-Correlation-Cust-Id") ? request.Headers["X-Correlation-Cust-Id"] : null;

                Console.WriteLine($"Headers: client_id={clientId}, User-Agent={userAgent}, Content-Type={contentType}, X-Correlation-Cust-Id={correlation}");

                if (string.IsNullOrEmpty(request.Body))
                {
                    return ErrorResponse(400, "Request body is required");
                }

                Request model;
                try
                {
                    model = JsonSerializer.Deserialize<Request>(request.Body, _jsonOptions) ?? new Request();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Deserialization error: {ex}");
                    return ErrorResponse(400, "Invalid JSON payload");
                }

                var validationErrors = _service.ValidateRequest(model);
                if (validationErrors != null && validationErrors.Count > 0)
                {
                    return ErrorResponse(400, "Validation failed", validationErrors);
                }

                var result = await _service.SaveAsync(model);

                var response = new Response
                {
                    railcardId = result.railcardId,
                    token = result.token
                };

                var body = JsonSerializer.Serialize(response, _jsonOptions);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 201,
                    Body = body,
                    Headers = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex}");
                return ErrorResponse(500, "Internal server error", null, ex.Message);
            }
        }

        private APIGatewayProxyResponse ErrorResponse(int statusCode, string message, System.Collections.Generic.List<string>? details = null, string? internalMessage = null)
        {
            var err = new
            {
                error = new
                {
                    message,
                    details = details,
                    internalMessage = internalMessage
                }
            };
            var body = JsonSerializer.Serialize(err, _jsonOptions);
            return new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = body,
                Headers = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };
        }
    }
}
