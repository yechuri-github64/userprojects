using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using TestsflambdaLambda.Models;
using TestsflambdaLambda.Services;

[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TestsflambdaLambda
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

            _service = new Service(config);
        }

        // Lambda handler must be named "testsflambda"
        public async Task<Response> testsflambda(Request request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogLine($"Received operation: {request.Operation}");

                switch (request.Operation?.ToLowerInvariant())
                {
                    case "create":
                        if (request.Accounts == null || request.Accounts.Count == 0)
                        {
                            return Response.FromError("No accounts provided for creation", "Provide an array of account objects in the 'Accounts' property.");
                        }
                        var created = await _service.CreateAccountsAsync(request.Accounts);
                        return Response.Ok(created);

                    case "retrieve":
                        if (string.IsNullOrWhiteSpace(request.Id))
                        {
                            return Response.FromError("No Id provided for retrieve", "Provide a valid Account Id in the 'Id' property.");
                        }
                        var retrieved = await _service.GetAccountAsync(request.Id);
                        return Response.Ok(retrieved);

                    case "update":
                        if (request.Accounts == null || request.Accounts.Count == 0)
                        {
                            return Response.FromError("No account provided for update", "Provide a single account object in the 'Accounts' array to update (one at a time).");
                        }
                        var accountToUpdate = request.Accounts[0];
                        if (string.IsNullOrWhiteSpace(accountToUpdate.Id))
                        {
                            return Response.FromError("No Id in account for update", "Account must include its 'Id' to update.");
                        }
                        await _service.UpdateAccountAsync(accountToUpdate);
                        return Response.Ok(new { message = "Account updated", id = accountToUpdate.Id });

                    case "delete":
                        if (string.IsNullOrWhiteSpace(request.Id))
                        {
                            return Response.FromError("No Id provided for delete", "Provide a valid Account Id in the 'Id' property.");
                        }
                        await _service.DeleteAccountAsync(request.Id);
                        return Response.Ok(new { message = "Account deleted", id = request.Id });

                    default:
                        return Response.FromError("Unsupported operation", "Supported operations are: create, retrieve, update, delete.");
                }
            }
            catch (ServiceException sx)
            {
                context.Logger.LogLine($"Service error: {sx.Message}");
                return Response.FromException(sx);
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Unhandled error: {ex}");
                return Response.FromException(new ServiceException(500, "Unhandled error", ex.Message));
            }
        }
    }
}
