using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using DatatableLambda.Services;
using DatatableLambda.Models;
using DatatableLambda.Data;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DatatableLambda
{
    public class Function
    {
        private readonly Service _service;

        public Function()
        {
            _service = new Service();
        }

        // Handler method must be named "datatable"
        public async Task<APIGatewayProxyResponse> datatable(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogLine("datatabe: received request");

                // Optional: parse incoming request body into Models.Request
                Request? req = null;
                if (!string.IsNullOrEmpty(request?.Body))
                {
                    try
                    {
                        req = JsonSerializer.Deserialize<Request>(request.Body);
                    }
                    catch (Exception ex)
                    {
                        context.Logger.LogLine($"Failed to deserialize request body: {ex}");
                        // continue with null request
                    }
                }

                var accounts = await _service.GetAccountsAsync();

                var response = new Response
                {
                    Success = true,
                    Message = "Accounts retrieved",
                    Data = accounts
                };

                var body = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = body,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error in datatable handler: {ex}");

                var errorResponse = new Response
                {
                    Success = false,
                    Message = "An error occurred while retrieving accounts",
                    Error = new ErrorDetail { Message = ex.Message, StackTrace = ex.StackTrace }
                };

                var body = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = body,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
        }
    }
}
