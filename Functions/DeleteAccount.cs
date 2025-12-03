using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Models;
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
 var err = new ErrorResponse { Code = "not_found", Message = $"Account with id {id} not found." };
 await response.WriteAsJsonAsync(err);
 return response;
 }

 response.StatusCode = HttpStatusCode.NoContent;
 return response;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "Error deleting account {Id}", id);
 response.StatusCode = HttpStatusCode.InternalServerError;
 var err = new ErrorResponse { Code = "internal_error", Message = "Failed to delete account.", Details = ex.Message };
 await response.WriteAsJsonAsync(err);
 return response;
 }
 }
 }
}