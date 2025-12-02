using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Functions
{
 public class GetAccount
 {
 private readonly IAccountService _accountService;

 public GetAccount(IAccountService accountService)
 {
 _accountService = accountService;
 }

 [Function("GetAccount")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{id}")] HttpRequestData req,
 string id)
 {
 var response = req.CreateResponse();
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 try
 {
 var account = await _accountService.GetAccountAsync(id).ConfigureAwait(false);
 if (account == null)
 {
 response.StatusCode = HttpStatusCode.NotFound;
 await response.WriteStringAsync(JsonSerializer.Serialize(new { error = "Account not found" }));
 return response;
 }

 response.StatusCode = HttpStatusCode.OK;
 await response.WriteStringAsync(JsonSerializer.Serialize(account, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
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