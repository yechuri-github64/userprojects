using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Services;

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
 try
 {
 var deleted = await _accountService.DeleteAccountAsync(id);
 if (!deleted)
 {
 response.StatusCode = HttpStatusCode.NotFound;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 var err = new { error = new { message = "Account not found" } };
 await response.WriteStringAsync(JsonSerializer.Serialize(err));
 return response;
 }

 response.StatusCode = HttpStatusCode.NoContent;
 return response;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "DeleteAccount failed for {Id}", id);
 response.StatusCode = HttpStatusCode.InternalServerError;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 var err = new { error = new { message = "Internal server error", details = ex.Message } };
 await response.WriteStringAsync(JsonSerializer.Serialize(err));
 return response;
 }
 }
 }
}