using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using CreateaccountsdemoLambda.Models;
using CreateaccountsdemoLambda.Services;

[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreateaccountsdemoLambda
{
    public class Function
    {
        private readonly Service _service;

        public Function()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();
            _service = new Service(config);
        }

        // Handler method name matches aws-lambda-tools-defaults.json
        public async Task<Response> createaccountsdemo(Request request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogLine($"Received request: Operation={request.Operation}");
                return await _service.ProcessAsync(request);
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Unhandled exception: {ex}");
                return new Response
                {
                    Success = false,
                    Error = new ErrorModel
                    {
                        Code = "UnhandledException",
                        Message = ex.Message,
                        Details = ex.ToString(),
                        Timestamp = DateTime.UtcNow
                    }
                };
            }
        }
    }
}
