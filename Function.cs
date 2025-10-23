using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using AccountslamdaLambda.Models;
using AccountslamdaLambda.Services;

[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AccountslamdaLambda
{
    public class Function
    {
        private readonly Service _service;

        public Function()
        {
            // Build configuration from appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            IConfiguration config = builder.Build();
            _service = new Service(config);
        }

        /// <summary>
        /// Lambda handler named "accountslamda" as required.
        /// Expects a JSON request with AccountId and returns the last created account for that account id.
        /// Errors are logged and returned in structured error format.
        /// </summary>
        public async Task<Response> accountslamda(Request request, ILambdaContext context)
        {
            var response = new Response();
            try
            {
                if (request == null)
                {
                    response.Success = false;
                    response.Error = new Response.ErrorResponse { Message = "Request body is null." };
                    context.Logger.LogLine("Request body is null.");
                    return response;
                }

                var account = await _service.GetLastCreatedAccountAsync(request.AccountId);
                if (account == null)
                {
                    response.Success = true;
                    response.Data = null;
                    return response;
                }

                response.Success = true;
                response.Data = account;
                return response;
            }
            catch (Exception ex)
            {
                // Log and return structured error
                context.Logger.LogLine($"Error in accountslamda: {ex}");
                response.Success = false;
                response.Error = new Response.ErrorResponse
                {
                    Message = ex.Message ?? "An unknown error occurred.",
                    Details = ex.ToString()
                };
                return response;
            }
        }
    }
}
