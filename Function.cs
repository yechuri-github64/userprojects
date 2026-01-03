using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using TestLambda.Models;
using TestLambda.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TestLambda
{
    public class Function
    {
        private readonly Service _service;

        public Function()
        {
            _service = new Service();
            Console.WriteLine("Function initialized");
        }

        // Lambda handler must be named "Test"
        public async Task<APIGatewayProxyResponse> Test(APIGatewayProxyRequest request, ILambdaContext context)
        {
            Console.WriteLine("Handler Test invoked");
            try
            {
                // Extract headers
                var headerInfo = request.Headers ?? new System.Collections.Generic.Dictionary<string, string>();
                if (headerInfo.TryGetValue("x-request-id", out var reqId))
                {
                    Console.WriteLine($"Request header x-request-id: {reqId}");
                }

                // Extract query parameters (if any)
                var queryParams = request.QueryStringParameters ?? new System.Collections.Generic.Dictionary<string, string>();
                if (queryParams.TryGetValue("dryRun", out var dryRunVal))
                {
                    Console.WriteLine($"Query param dryRun: {dryRunVal}");
                }

                // Parse body
                if (string.IsNullOrWhiteSpace(request.Body))
                {
                    var err = new ErrorResponse(new[] { "Empty request body" });
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 400,
                        Body = JsonSerializer.Serialize(err),
                        Headers = new System.Collections.Generic.Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var req = JsonSerializer.Deserialize<Request>(request.Body, options);
                if (req == null)
                {
                    var err = new ErrorResponse(new[] { "Invalid request body" });
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 400,
                        Body = JsonSerializer.Serialize(err),
                        Headers = new System.Collections.Generic.Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };
                }

                // Validate request
                var validation = _service.ValidateRequest(req);
                if (!validation.IsValid)
                {
                    Console.WriteLine("Validation failed");
                    var err = new ErrorResponse(validation.Errors);
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 400,
                        Body = JsonSerializer.Serialize(err),
                        Headers = new System.Collections.Generic.Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };
                }

                // Process
                var response = await _service.ProcessTeamAsync(req);

                if (!response.Success)
                {
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 500,
                        Body = JsonSerializer.Serialize(response),
                        Headers = new System.Collections.Generic.Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };
                }

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(response),
                    Headers = new System.Collections.Generic.Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex}");
                var err = new ErrorResponse(new[] { ex.Message });
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = JsonSerializer.Serialize(err),
                    Headers = new System.Collections.Generic.Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
        }
    }
}
