using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services;
using System.Net;
using Microsoft.Extensions.Logging;

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
 catch (Exception ex)
 {
 _logger.LogError(ex, "Unhandled error in ListAccounts");
 response.StatusCode = HttpStatusCode.InternalServerError;
 await response.WriteAsJsonAsync(new { error = "internal_error", details = "An unexpected error occurred" });
 return response;
 }
 }
 }
}