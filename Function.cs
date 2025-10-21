using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using DatatableLambda.Models;
using DatatableLambda.Services;

[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DatatableLambda
{
    public class Function
    {
        private readonly Service _service;

        public Function()
        {
            // Service will load env and configure DbContext
            _service = new Service();
        }

        /// <summary>
        /// Lambda handler method named exactly "datatable"
        /// </summary>
        public async Task<Response> datatable(Request request, ILambdaContext context)
        {
            try
            {
                var accounts = await _service.GetAllAccountsAsync();
                return new Response
                {
                    Success = true,
                    ErrorMessage = null,
                    Accounts = accounts
                };
            }
            catch (Exception ex)
            {
                // Log and return structured error
                try
                {
                    context.Logger.LogLine($"Error in datatable handler: {ex.Message}");
                    context.Logger.LogLine(ex.StackTrace ?? string.Empty);
                }
                catch { }

                return new Response
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    Accounts = null
                };
            }
        }
    }
}
