using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using AccountslamdaLambda.Models;
using AccountslamdaLambda.Services;
using Amazon.Lambda.Serialization.SystemTextJson;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace AccountslamdaLambda
{
    public class Function
    {
        private readonly Service _service = new Service();

        public async Task<Response> accountslamda(Request request, ILambdaContext context)
        {
            try
            {
                if (request == null)
                {
                    var err = new ErrorResponse { Code = "InvalidRequest", Message = "Request is null" };
                    context.Logger.LogLine("Request was null.");
                    return new Response { Success = false, Data = null, Error = err };
                }

                var result = await _service.GetLastAccountByIdAsync(request.Id, context);
                return new Response { Success = true, Data = result, Error = null };
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Exception: {ex}");
                var err = new ErrorResponse { Code = "ServerError", Message = ex.Message, Details = ex.ToString() };
                return new Response { Success = false, Data = null, Error = err };
            }
        }
    }
}
