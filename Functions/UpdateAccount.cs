using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Models;
using Services;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Functions
{
 public class UpdateAccount
 {
 private readonly IAccountService _accountService;

 public UpdateAccount(IAccountService accountService)
 {
 _accountService = accountService;
 }

 [Function("UpdateAccount")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "put", Route = "accounts/{id}")] HttpRequestData req,
 string id)
 {
 var response = req.CreateResponse();
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 try
 {
 using var reader = new StreamReader(req.Body);
 var body = await reader.ReadToEndAsync().ConfigureAwait(false);
 if (string.IsNullOrWhiteSpace(body))
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 await response.WriteStringAsync(JsonSerializer.Serialize(new { error = "Request body is empty" }));
 return response;
 }

 var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
 var account = JsonSerializer.Deserialize<Account>(body, options);
 if (account == null)
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 await response.WriteStringAsync(JsonSerializer.Serialize(new { error = "Invalid account payload" }));
 return response;
 }

 var updated = await _accountService.UpdateAccountAsync(id, account).ConfigureAwait(false);
 if (updated == null)
 {
 response.StatusCode = HttpStatusCode.NotFound;
 await response.WriteStringAsync(JsonSerializer.Serialize(new { error = "Account not found or update failed" }));
 return response;
 }

 response.StatusCode = HttpStatusCode.OK;
 await response.WriteStringAsync(JsonSerializer.Serialize(updated, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
 return response;
 }
 catch (System.Exception ex)
 {
 response.StatusCode = HttpStatusCode.InternalServerError;
 await response.WriteStringAsync(JsonSerializer.Serialize(new { error = ex.Message }));
 return response;
 }
 }
 }
}