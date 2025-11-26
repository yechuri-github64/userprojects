using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using CreaterailcardLambda.Models;
using CreaterailcardLambda.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreaterailcardLambda
{
    public class Function
    {
        private readonly Service _service;

        public Function()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            var connectionString = configuration.GetSection("ConnectionStrings")?[
                "PostgreSql"] ?? throw new InvalidOperationException("PostgreSql connection string not configured");

            _service = new Service(connectionString);
        }

        // Lambda handler must match function-handler in aws-lambda-tools-defaults.json (createrailcard)
        public async Task<Response> createrailcard(Request request, ILambdaContext context)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                var response = await _service.CreateRailcardAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error: {ex.Message}");
                return new Response
                {
                    Error = new ErrorResponse
                    {
                        Message = ex.Message,
                        Detail = ex.StackTrace
                    }
                };
            }
        }
    }
}
