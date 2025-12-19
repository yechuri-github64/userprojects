using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using CreatetravelcardLambda.Models;
using CreatetravelcardLambda.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreatetravelcardLambda;

public class Function
{
    private readonly Service _service;

    public Function()
    {
        Console.WriteLine("Initializing Createtravelcard Lambda");
        _service = new Service();
    }

    /// <summary>
    /// Lambda entry point - method must be named Createtravelcard
    /// </summary>
    public async Task<APIGatewayProxyResponse> Createtravelcard(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            Console.WriteLine("Received request");

            // Extract headers
            request.Headers ??= new Dictionary<string, string>();
            request.QueryStringParameters ??= new Dictionary<string, string>();

            request.Headers.TryGetValue("client_id", out var clientId);
            request.Headers.TryGetValue("Content-Type", out var contentType);
            request.Headers.TryGetValue("X-Correlation-Cust-Id", out var correlationId);

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return _service.CreateErrorResponse(400, "Missing required header: client_id");
            }

            if (!string.Equals(contentType, "application/json", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(request.Body))
            {
                return _service.CreateErrorResponse(400, "Content-Type must be application/json for requests with a body");
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            jsonOptions.Converters.Add(new JsonStringEnumConverter());

            var req = JsonSerializer.Deserialize<Request>(request.Body ?? "{}", jsonOptions);
            if (req is null)
            {
                return _service.CreateErrorResponse(400, "Invalid request payload");
            }

            // Validate
            var validationErrors = _service.ValidateRequest(req);
            if (validationErrors.Any())
            {
                return _service.CreateErrorResponse(400, "Validation failed", validationErrors);
            }

            // Persist
            var result = await _service.CreateTravelcardAsync(req);

            var resp = new Response
            {
                TravelcardId = result.travelcardId,
                Token = result.token
            };

            var body = JsonSerializer.Serialize(resp);
            return new APIGatewayProxyResponse
            {
                StatusCode = 201,
                Body = body,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "X-Correlation-Cust-Id", correlationId ?? string.Empty }
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled exception: {ex}");
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = System.Text.Json.JsonSerializer.Serialize(new { error = "Internal server error", detail = ex.Message }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
