using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Models;
using Services;
using System.Net;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Functions
{
 public class CreateAccounts
 {
 private readonly IAccountService _accountService;

 public CreateAccounts(IAccountService accountService)
 {
 _accountService = accountService;
 }

 [Function("CreateAccounts")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/batch")] HttpRequestData req)
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
 var doc = JsonSerializer.Deserialize<JsonElement>(body, options);
 List<Account>? accounts = null;

 if (doc.ValueKind == JsonValueKind.Object && doc.TryGetProperty("accounts", out var accountsProp) && accountsProp.ValueKind == JsonValueKind.Array)
 {
 accounts = JsonSerializer.Deserialize<List<Account>>(accountsProp.GetRawText(), options);
 }
 else
 {
 // Assume body is an array of accounts
 accounts = JsonSerializer.Deserialize<List<Account>>(body, options);
 }

 if (accounts == null || accounts.Count == 0)
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 await response.WriteStringAsync(JsonSerializer.Serialize(new { error = "No accounts provided" }));
 return response;
 }

 var created = await _accountService.CreateAccountsAsync(accounts).ConfigureAwait(false);
 response.StatusCode = HttpStatusCode.Created;
 await response.WriteStringAsync(JsonSerializer.Serialize(new { created = created }, options));
 return response;
 }
 catch (Exception ex)
 {
 response.StatusCode = HttpStatusCode.InternalServerError;
 await response.WriteStringAsync(JsonSerializer.Serialize(new { error = ex.Message }));
 return response;
 }
 }
 }
}