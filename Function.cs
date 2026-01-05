using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using DemotestprojLambda.Models;
using DemotestprojLambda.Services;

#nullable enable

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DemotestprojLambda
{
    public class Function
    {
        private readonly Service _service;
        private readonly JsonSerializerOptions _jsonOptions;

        public Function()
        {
            _service = new Service();
            _jsonOptions = Service.CreateJsonOptions();
        }

        // Lambda handler name must be Demotestproj as requested
        public async Task<APIGatewayProxyResponse> demotestproj(APIGatewayProxyRequest request, ILambdaContext context)
        {
            Console.WriteLine("Function invoked");

            try
            {
                // Extract headers
                string? clientId = null;
                string? contentType = null;
                string? correlationId = null;
                request.Headers?.TryGetValue("client_id", out var clientId);
                request.Headers?.TryGetValue("Content-Type", out var contentType);
                request.Headers?.TryGetValue("X-Correlation-Cust-Id", out var correlationId);

                Console.WriteLine($"Headers: client_id={clientId}, Content-Type={contentType}, X-Correlation-Cust-Id={correlationId}");

                if (string.IsNullOrWhiteSpace(request.Body))
                {
                    return CreateErrorResponse(400, "Missing body");
                }

                var reqModel = JsonSerializer.Deserialize<Request>(request.Body, _jsonOptions);
                if (reqModel == null)
                {
                    return CreateErrorResponse(400, "Invalid JSON payload");
                }

                var validationErrors = _service.ValidateRequest(reqModel);
                if (validationErrors.Length > 0)
                {
                    return CreateErrorResponse(400, "Validation failed", validationErrors);
                }

                // Insert into DB
                var result = await _service.CreateTravelcardAsync(reqModel);

                var response = new Response
                {
                    travelcardId = result.travelcardId,
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
                Console.WriteLine($"Unhandled error: {ex}");
                return CreateErrorResponse(500, "Internal server error", new[] { ex.Message });
            }
        }

        private APIGatewayProxyResponse CreateErrorResponse(int statusCode, string message, string[]? details = null)
        {
            var err = new { error = new { message, details } };
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
