using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Functions
{
 public class ListAccounts
 {
 private readonly IAccountService _accountService;

 public ListAccounts(IAccountService accountService)
 {
 _accountService = accountService;
 }

 [Function("ListAccounts")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts")] HttpRequestData req)
 {
 var response = req.CreateResponse();
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 try
 {
 var accounts = await _accountService.ListAccountsAsync().ConfigureAwait(false);
 response.StatusCode = HttpStatusCode.OK;
 await response.WriteStringAsync(JsonSerializer.Serialize(accounts, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
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