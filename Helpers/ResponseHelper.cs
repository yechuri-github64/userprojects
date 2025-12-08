using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;

namespace SalesforceAccountFunctions.Helpers
{
 public static class ResponseHelper
 {
 public static async Task<HttpResponseData> CreateJsonResponseAsync(HttpRequestData req, object body, int statusCode = 200)
 {
 var res = req.CreateResponse((System.Net.HttpStatusCode)statusCode);
 res.Headers.Add("Content-Type", "application/json");
 await res.WriteStringAsync(JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
 return res;
 }

 public static async Task<HttpResponseData> CreateErrorResponseAsync(HttpRequestData req, string message, string code = "error", int statusCode = 400)
 {
 var err = new { error = new { code, message } };
 return await CreateJsonResponseAsync(req, err, statusCode);
 }
 }
}
