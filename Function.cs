using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using TestMyCardLambda.Models;
using TestMyCardLambda.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TestMyCardLambda
{
    public class Function
    {
        private readonly Service _service;

        public Function()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var conn = config.GetConnectionString("PostgreSql") ?? string.Empty;
            _service = new Service(conn);
        }

        public async Task<Response> TestMyCard(Request request, ILambdaContext context)
        {
            try
            {
                var result = await _service.CreateOrderAsync(request);
                if (!result.Success)
                {
                    context.Logger.LogLine($"Operation failed: {result.ErrorMessage}");
                }
                return result;
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Unhandled error: {ex}");
                return new Response
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
