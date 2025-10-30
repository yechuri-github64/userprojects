using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.Extensions.Configuration;
using GetaccountsLambda.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GetaccountsLambda
{
    public class Function
    {
        private readonly Service _service;
        private readonly IConfiguration _configuration;

        public Function()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _configuration = builder.Build();
            _service = new Service(_configuration);
        }

        // Lambda handler must be named "getaccounts"
        public async Task<APIGatewayProxyResponse> getaccounts(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                string? accountId = null;

                if (request?.PathParameters != null && request.PathParameters.ContainsKey("accountid"))
                    accountId = request.PathParameters["accountid"];

                if (string.IsNullOrWhiteSpace(accountId) && request?.QueryStringParameters != null && request.QueryStringParameters.ContainsKey("accountid"))
                    accountId = request.QueryStringParameters["accountid"];

                if (string.IsNullOrWhiteSpace(accountId))
                {
                    var err = new { error = new { message = "accountid not provided in path or querystring", type = "BadRequest" } };
                    var errJson = JsonSerializer.Serialize(err);
                    context.Logger.LogLine(errJson);
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 400,
                        Body = errJson,
                        Headers = new Dictionary<string, string> {{ "Content-Type", "application/json" }}
                    };
                }

                var account = await _service.GetAccountByIdAsync(accountId, context);
                var json = JsonSerializer.Serialize(account, new JsonSerializerOptions { WriteIndented = false });

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = json,
                    Headers = new Dictionary<string, string> {{ "Content-Type", "application/json" }}
                };
            }
            catch (Exception ex)
            {
                var errorObj = new { error = new { message = ex.Message, type = ex.GetType().Name, details = ex.StackTrace } };
                var errorJson = JsonSerializer.Serialize(errorObj);
                context.Logger.LogLine(errorJson);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = errorJson,
                    Headers = new Dictionary<string, string> {{ "Content-Type", "application/json" }}
                };
            }
        }
    }
}
