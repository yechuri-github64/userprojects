using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Models;
using Services;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Functions
{
 public class UpdateAccount
 {
 private readonly IAccountService _accountService;
 private readonly ILogger<UpdateAccount> _logger;

 public UpdateAccount(IAccountService accountService, ILogger<UpdateAccount> logger)
 {
 _accountService = accountService;
 _logger = logger;
 }

 [Function("UpdateAccount")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "put", Route = "accounts/{id}")] HttpRequestData req,
 string id)
 {
 var response = req.CreateResponse();
 if (string.IsNullOrWhiteSpace(id))
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 await response.WriteAsJsonAsync(new { error = "invalid_id", details = "Account id is required" });
 return response;
 }

 try
 {
 var body = await new StreamReader(req.Body).ReadToEndAsync();
 if (string.IsNullOrWhiteSpace(body))
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 await response.WriteAsJsonAsync(new { error = "invalid_request", details = "Request body must contain account data" });
 return response;
 }

 Account? account = null;
 try
 {
 account = JsonSerializer.Deserialize<Account>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
 }
 catch (JsonException jex)
 {
 _logger.LogWarning(jex, "Invalid JSON in update request");
 response.StatusCode = HttpStatusCode.BadRequest;
 await response.WriteAsJsonAsync(new { error = "Invalid JSON", details = jex.Message });
 return response;
 }

 if (account == null)
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 await response.WriteAsJsonAsync(new { error = "invalid_request", details = "Request body must contain account data" });
 return response;
 }

 var updated = await _accountService.UpdateAccountAsync(id, account);
 if (updated == null)
 {
 response.StatusCode = HttpStatusCode.NotFound;
 await response.WriteAsJsonAsync(new { error = "not_found", details = "Account not found or could not be updated" });
 return response;
 }

 response.StatusCode = HttpStatusCode.OK;
 await response.WriteAsJsonAsync(updated);
 return response;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Unhandled error in UpdateAccount {Id}", id);
 response.StatusCode = HttpStatusCode.InternalServerError;
 await response.WriteAsJsonAsync(new { error = "internal_error", details = "An unexpected error occurred" });
 return response;
 }
 }
 }
}