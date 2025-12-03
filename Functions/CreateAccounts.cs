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
 public class CreateAccounts
 {
 private readonly IAccountService _accountService;
 private readonly ILogger<CreateAccounts> _logger;

 public CreateAccounts(IAccountService accountService, ILogger<CreateAccounts> logger)
 {
 _accountService = accountService;
 _logger = logger;
 }

 [Function("CreateAccounts")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/batch")] HttpRequestData req)
 {
 var response = req.CreateResponse();
 try
 {
 var body = await new StreamReader(req.Body).ReadToEndAsync();
 if (string.IsNullOrWhiteSpace(body))
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 await response.WriteAsJsonAsync(new { error = "Invalid request", details = "Request body must be a non-empty JSON array of accounts" });
 return response;
 }

 List<Account>? accounts = null;
 try
 {
 accounts = JsonSerializer.Deserialize<List<Account>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
 }
 catch (JsonException jex)
 {
 _logger.LogWarning(jex, "Invalid JSON in request body");
 response.StatusCode = HttpStatusCode.BadRequest;
 await response.WriteAsJsonAsync(new { error = "Invalid JSON", details = jex.Message });
 return response;
 }

 if (accounts == null || accounts.Count == 0)
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 await response.WriteAsJsonAsync(new { error = "Invalid request", details = "Request body must be a non-empty JSON array of accounts" });
 return response;
 }

 var created = await _accountService.CreateAccountsAsync(accounts);
 response.StatusCode = HttpStatusCode.Created;
 await response.WriteAsJsonAsync(created);
 return response;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Unhandled error in CreateAccounts");
 response.StatusCode = HttpStatusCode.InternalServerError;
 await response.WriteAsJsonAsync(new { error = "internal_error", details = "An unexpected error occurred" });
 return response;
 }
 }
 }
}