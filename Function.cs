using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ManageordersLambda
{
    public class Function
    {
        private readonly Services.Service _service;

        public Function()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _service = new Services.Service(config);
        }

        /// <summary>
        /// Lambda handler. Function name referenced in aws-lambda-tools-defaults.json: manageorders
        /// Supports actions: "retrieve" and "update"
        /// </summary>
        public async Task<Models.Response> manageorders(Models.Request request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogLine("manageorders invoked");

                if (request is null)
                {
                    context.Logger.LogLine("Request is null");
                    return new Models.Response
                    {
                        Success = false,
                        Error = new Models.ErrorModel { Code = "InvalidRequest", Message = "Request cannot be null" }
                    };
                }

                var action = (request.Action ?? string.Empty).Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(action))
                {
                    return new Models.Response
                    {
                        Success = false,
                        Error = new Models.ErrorModel { Code = "InvalidRequest", Message = "Action must be provided (retrieve or update)" }
                    };
                }

                switch (action)
                {
                    case "retrieve":
                        if (string.IsNullOrEmpty(request.OrderId))
                        {
                            return new Models.Response
                            {
                                Success = false,
                                Error = new Models.ErrorModel { Code = "InvalidRequest", Message = "OrderId must be provided for retrieve" }
                            };
                        }

                        return await _service.RetrieveOrderAsync(request.OrderId!, context);

                    case "update":
                        if (string.IsNullOrEmpty(request.OrderId))
                        {
                            return new Models.Response
                            {
                                Success = false,
                                Error = new Models.ErrorModel { Code = "InvalidRequest", Message = "OrderId must be provided for update" }
                            };
                        }

                        if (request.Data == null)
                        {
                            return new Models.Response
                            {
                                Success = false,
                                Error = new Models.ErrorModel { Code = "InvalidRequest", Message = "Data must be provided for update" }
                            };
                        }

                        return await _service.UpdateOrderAsync(request.OrderId!, request.Data.Value, context);

                    default:
                        return new Models.Response
                        {
                            Success = false,
                            Error = new Models.ErrorModel { Code = "InvalidAction", Message = $"Unsupported action: {request.Action}" }
                        };
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Unhandled exception: {ex}");

                return new Models.Response
                {
                    Success = false,
                    Error = new Models.ErrorModel { Code = "Exception", Message = ex.Message }
                };
            }
        }
    }
}
