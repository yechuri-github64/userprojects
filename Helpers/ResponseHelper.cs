using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace Helpers
{
 public static class ResponseHelper
 {
 private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

 public static HttpResponseData CreateErrorResponse(HttpRequestData req, HttpStatusCode statusCode, string errorCode, string message, object? details = null)
 {
 var resp = req.CreateResponse(statusCode);
 var error = new { errorCode, message, details };
 resp.Headers.Add("Content-Type", "application/json");
 var json = JsonSerializer.Serialize(error, JsonOptions);
 resp.WriteString(json);
 return resp;
 }
 }
}
