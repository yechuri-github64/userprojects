using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using AccountslamdaLambda.Services;
using AccountslamdaLambda.Models;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace AccountslamdaLambda
{
    public class Function
    {
        private readonly Service _service;

        public Function()
        {
            // Build configuration to read connection strings from appsettings.json or environment variables
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            _service = new Service(config);
        }

        /// <summary>Lambda handler must be named "accountslamda"</summary>
        public async Task<Response> accountslamda(Request? request, ILambdaContext context)
        {
            try
            {
                if (request == null)
                {
                    return new Response
                    {
                        Success = false,
                        Error = new Response.ErrorDetail
                        {
                            Message = "Invalid request",
                            Details = "Request body was null or not provided."
                        }
                    };
                }

                var account = await _service.GetLastCreatedAccountByIdAsync(request.Id);

                if (account == null)
                {
                    return new Response
                    {
                        Success = true,
                        Data = null
                    };
                }

                return new Response
                {
                    Success = true,
                    Data = account
                };
            }
            catch (Exception ex)
            {
                // Log error to stderr (CloudWatch)
                Console.Error.WriteLine(ex.ToString());

                return new Response
                {
                    Success = false,
                    Error = new Response.ErrorDetail
                    {
                        Message = "An error occurred while processing the request.",
                        Details = ex.Message,
                        StackTrace = ex.StackTrace
                    }
                };
            }
        }
    }
}
