using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Functions
{
 public class DeleteAccount
 {
 private readonly IAccountService _accountService;

 public DeleteAccount(IAccountService accountService)
 {
 _accountService = accountService;
 }

 [Function("DeleteAccount")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "accounts/{id}")] HttpRequestData req,
 string id)
 {
 var response = req.CreateResponse();
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 try
 {
 var deleted = await _accountService.DeleteAccountAsync(id).ConfigureAwait(false);
 if (!deleted)
 {
 response.StatusCode = HttpStatusCode.NotFound;
 await response.WriteStringAsync(JsonSerializer.Serialize(new { error = "Account not found or could not be deleted" }));
 return response;
 }

 response.StatusCode = HttpStatusCode.NoContent;
 return response;
 }
 catch (System.Exception ex)
 {
 response.StatusCode = HttpStatusCode.InternalServerError;
 await response.WriteStringAsync(JsonSerializer.Serialize(new { error = ex.Message }));
 return response;
 }
 }
 }
}