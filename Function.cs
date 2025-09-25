using System;
using System.Collections.Generic;
using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Test_project_mainLambda
{
    public class Function
    {
        private static readonly Random _rand = new Random();

        // Lambda handler method required name
        public APIGatewayProxyResponse test_project_main(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                // Defaults
                int min = 200;
                int max = 500;

                // Read query parameters if provided
                if (request?.QueryStringParameters != null)
                {
                    if (request.QueryStringParameters.TryGetValue("min", out var smin))
                    {
                        if (!int.TryParse(smin, out min))
                        {
                            var errBadMin = new ErrorResponse { Error = "InvalidParameter", Details = $"Could not parse 'min' value: '{smin}'" };
                            var badMinBody = JsonSerializer.Serialize(errBadMin);
                            context?.Logger.LogLine(badMinBody);
                            return new APIGatewayProxyResponse
                            {
                                StatusCode = 400,
                                Body = badMinBody,
                                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                            };
                        }
                    }

                    if (request.QueryStringParameters.TryGetValue("max", out var smax))
                    {
                        if (!int.TryParse(smax, out max))
                        {
                            var errBadMax = new ErrorResponse { Error = "InvalidParameter", Details = $"Could not parse 'max' value: '{smax}'" };
                            var badMaxBody = JsonSerializer.Serialize(errBadMax);
                            context?.Logger.LogLine(badMaxBody);
                            return new APIGatewayProxyResponse
                            {
                                StatusCode = 400,
                                Body = badMaxBody,
                                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                            };
                        }
                    }
                }

                if (min > max)
                {
                    var err = new ErrorResponse { Error = "InvalidRange", Details = $"Parameter 'min' ({min}) is greater than 'max' ({max})." };
                    var body = JsonSerializer.Serialize(err);
                    context?.Logger.LogLine(body);
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 400,
                        Body = body,
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };
                }

                // Generate random number inclusive
                int number = _rand.Next(min, max + 1);
                var resp = new ResponseData { Number = number };
                var json = JsonSerializer.Serialize(resp);

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = json,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse { Error = "InternalError", Details = ex.Message };
                var body = JsonSerializer.Serialize(err);
                context?.Logger.LogLine(body);
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
