using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using TestSfLambdaLambda.Models;
using TestSfLambdaLambda.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TestSfLambdaLambda
{
    public class Function
    {
        private readonly Service _service;
        private readonly IConfiguration _configuration;

        public Function()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
            _configuration = builder.Build();
            _service = new Service(_configuration);
        }

        // AWS function-handler configured in aws-lambda-tools-defaults.json as "test-sf-lambda"
        // Because C# identifiers cannot contain hyphens, this implementation exposes a valid identifier
        // and relies on the aws-lambda-tools-defaults.json handler mapping.
        public async Task<Response> test_sf_lambda(Request request, ILambdaContext context)
        {
            context.Logger.Log($"Request received: {JsonSerializer.Serialize(request)}");
            try
            {
                if (request == null)
                {
                    return new Response(false, null, new ErrorDetail("InvalidRequest", "Request is null"));
                }

                var op = request.Operation?.Trim().ToLowerInvariant();
                switch (op)
                {
                    case "create":
                        return await _service.CreateAccountsAsync(request, context);
                    case "get":
                        return await _service.GetAccountsAsync(request, context);
                    case "update":
                        return await _service.UpdateAccountAsync(request, context);
                    case "delete":
                        return await _service.DeleteAccountAsync(request, context);
                    default:
                        return new Response(false, null, new ErrorDetail("InvalidOperation", "Operation must be create/get/update/delete"));
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Unhandled exception: {ex}");
                return new Response(false, null, new ErrorDetail("UnhandledException", ex.Message));
            }
        }
    }
}
