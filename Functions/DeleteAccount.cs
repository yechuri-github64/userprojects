using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Functions
{
 public class DeleteAccount
 {
 private readonly IAccountService _accountService;
 private readonly ILogger<DeleteAccount> _logger;

 public DeleteAccount(IAccountService accountService, ILogger<DeleteAccount> logger)
 {
 _accountService = accountService;
 _logger = logger;
 }

 [Function("DeleteAccount")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "accounts/{id}")] HttpRequestData req,
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
 var deleted = await _accountService.DeleteAccountAsync(id);
 if (!deleted)
 {
 response.StatusCode = HttpStatusCode.NotFound;
 await response.WriteAsJsonAsync(new { error = "not_found", details = "Account not found" });
 return response;
 }

 response.StatusCode = HttpStatusCode.NoContent;
 return response;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Unhandled error in DeleteAccount {Id}", id);
 response.StatusCode = HttpStatusCode.InternalServerError;
 await response.WriteAsJsonAsync(new { error = "internal_error", details = "An unexpected error occurred" });
 return response;
 }
 }
 }
}