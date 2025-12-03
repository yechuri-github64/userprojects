using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Models;
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
 var err = new ErrorResponse { Code = "not_found", Message = $"Account with id {id} not found." };
 await response.WriteAsJsonAsync(err);
 return response;
 }
 response.StatusCode = HttpStatusCode.OK;
 await response.WriteAsJsonAsync(account);
 return response;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "Error fetching account {Id}", id);
 response.StatusCode = HttpStatusCode.InternalServerError;
 var err = new ErrorResponse { Code = "internal_error", Message = "Failed to retrieve account.", Details = ex.Message };
 await response.WriteAsJsonAsync(err);
 return response;
 }
 }
 }
}