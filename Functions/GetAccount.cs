using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Functions
{
 public class GetAccount
 {
 private readonly IAccountService _accountService;
 private readonly ILogger<GetAccount> _logger;

 public GetAccount(IAccountService accountService, ILogger<GetAccount> logger)
 {
 _accountService = accountService;
 _logger = logger;
 }

 [Function("GetAccount")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{id}")] HttpRequestData req,
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
 var account = await _accountService.GetAccountAsync(id);
 if (account == null)
 {
 response.StatusCode = HttpStatusCode.NotFound;
 await response.WriteAsJsonAsync(new { error = "not_found", details = "Account not found" });
 return response;
 }

 response.StatusCode = HttpStatusCode.OK;
 await response.WriteAsJsonAsync(account);
 return response;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error getting account {Id}", id);
 response.StatusCode = HttpStatusCode.InternalServerError;
 await response.WriteAsJsonAsync(new { error = "internal_error", details = "An unexpected error occurred" });
 return response;
 }
 }
 }
}