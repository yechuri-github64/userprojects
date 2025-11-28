using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Services;

namespace Functions
{
 public class ListAccounts
 {
 private readonly IAccountService _accountService;
 private readonly ILogger<ListAccounts> _logger;

 public ListAccounts(IAccountService accountService, ILogger<ListAccounts> logger)
 {
 _accountService = accountService;
 _logger = logger;
 }

 [Function("ListAccounts")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts")] HttpRequestData req)
 {
 var response = req.CreateResponse();
 try
 {
 var accounts = await _accountService.ListAccountsAsync();
 response.StatusCode = HttpStatusCode.OK;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 await response.WriteStringAsync(JsonSerializer.Serialize(accounts));
 return response;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "ListAccounts failed");
 response.StatusCode = HttpStatusCode.InternalServerError;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 var err = new { error = new { message = "Internal server error", details = ex.Message } };
 await response.WriteStringAsync(JsonSerializer.Serialize(err));
 return response;
 }
 }
 }
}