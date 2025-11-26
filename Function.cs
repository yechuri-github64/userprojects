using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CreateaccountsdemoLambda.Services;
using CreateaccountsdemoLambda.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreateaccountsdemoLambda
{
    public class Function
    {
        private readonly Service _service;

        public Function()
        {
            _service = new Service();
        }

        // Lambda handler method name must be Createaccountsdemo
        public async Task<Response> Createaccountsdemo(Request request, ILambdaContext context)
        {
            try
            {
                if (request == null)
                {
                    context.Logger.LogLine("Request is null");
                    return Response.ErrorResponse("Request is null");
                }

                switch (request.Action)
                {
                    case ActionType.Get:
                        return await _service.GetAccountAsync(request, context);
                    case ActionType.Create:
                        return await _service.CreateAccountsAsync(request, context);
                    case ActionType.Update:
                        return await _service.UpdateAccountAsync(request, context);
                    case ActionType.Delete:
                        return await _service.DeleteAccountAsync(request, context);
                    default:
                        return Response.ErrorResponse("Unsupported action");
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Unhandled exception: {ex}");
                return Response.FromException(ex);
            }
        }
    }
}
