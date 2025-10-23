using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using ContractmanagementemailLambda.Models;
using ContractmanagementemailLambda.Services;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace ContractmanagementemailLambda
{
    public class Function
    {
        private readonly Service _service;
        private readonly ILambdaLogger _logger;

        public Function()
        {
            string conn = "dummy";
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("ConnectionStrings", out var cs) &&
                        cs.TryGetProperty("MySQL", out var mysql))
                    {
                        conn = mysql.GetString() ?? "dummy";
                    }
                }
            }
            catch
            {
                // fall back to dummy
            }

            _service = new Service(conn, _logger);
        }

        /// <summary>
        /// Lambda handler. Accepts a Request object. Use Action="enqueue" to read from DB and push into queue.
        /// Use any other or omit Action to fetch an object from the queue and return it as JSON.
        /// Errors are logged and returned in the structured error format on the Response.Error property.
        /// </summary>
        public async Task<Response> contractmanagementemail(Request request, ILambdaContext context)
        {
            try
            {
                var action = request?.Action?.Trim().ToLowerInvariant() ?? string.Empty;
                if (action == "enqueue")
                {
                    await _service.EnqueueFromDbAsync(s => context.Logger.LogLine(s));
                    // return success with no data
                    return new Response { Data = null };
                }

                var res = _service.GetFromQueue();
                if (res.Error != null)
                {
                    context.Logger.LogLine($"Returning error: {res.Error.Code} - {res.Error.Message}");
                }
                else
                {
                    context.Logger.LogLine($"Returning data: {JsonSerializer.Serialize(res.Data)}");
                }

                return res;
            }
            catch (Exception ex)
            {
                context.Logger.LogLine(ex.ToString());
                return new Response
                {
                    Error = new Response.ErrorResponse
                    {
                        Code = "InternalError",
                        Message = ex.Message,
                        Details = ex.ToString()
                    }
                };
            }
        }
    }
}
