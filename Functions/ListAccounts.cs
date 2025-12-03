using System.Net;
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
 await response.WriteAsJsonAsync(accounts);
 return response;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "Error listing accounts");
 response.StatusCode = HttpStatusCode.InternalServerError;
 var err = new Models.ErrorResponse { Code = "internal_error", Message = "Failed to list accounts.", Details = ex.Message };
 await response.WriteAsJsonAsync(err);
 return response;
 }
 }
 }
}