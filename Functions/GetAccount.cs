using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Services;

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
 try
 {
 var account = await _accountService.GetAccountAsync(id);
 if (account == null)
 {
 response.StatusCode = HttpStatusCode.NotFound;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 var err = new { error = new { message = "Account not found" } };
 await response.WriteStringAsync(JsonSerializer.Serialize(err));
 return response;
 }

 response.StatusCode = HttpStatusCode.OK;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 await response.WriteStringAsync(JsonSerializer.Serialize(account));
 return response;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "GetAccount failed for {Id}", id);
 response.StatusCode = HttpStatusCode.InternalServerError;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 var err = new { error = new { message = "Internal server error", details = ex.Message } };
 await response.WriteStringAsync(JsonSerializer.Serialize(err));
 return response;
 }
 }
 }
}