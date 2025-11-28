using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Models;
using Services;

namespace Functions
{
 public class UpdateAccount
 {
 private readonly IAccountService _accountService;
 private readonly ILogger<UpdateAccount> _logger;

 public UpdateAccount(IAccountService accountService, ILogger<UpdateAccount> logger)
 {
 _accountService = accountService;
 _logger = logger;
 }

 [Function("UpdateAccount")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "put", Route = "accounts/{id}")] HttpRequestData req,
 string id)
 {
 var response = req.CreateResponse();
 try
 {
 var body = await new StreamReader(req.Body).ReadToEndAsync();
 var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
 var account = JsonSerializer.Deserialize<Account>(body, options);
 if (account == null)
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 var err = new { error = new { message = "Invalid account payload" } };
 await response.WriteStringAsync(JsonSerializer.Serialize(err));
 return response;
 }

 var updated = await _accountService.UpdateAccountAsync(id, account);
 if (updated == null)
 {
 response.StatusCode = HttpStatusCode.NotFound;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 var err = new { error = new { message = "Account not found" } };
 await response.WriteStringAsync(JsonSerializer.Serialize(err));
 return response;
 }

 response.StatusCode = HttpStatusCode.OK;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 await response.WriteStringAsync(JsonSerializer.Serialize(updated, options));
 return response;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "UpdateAccount failed for {Id}", id);
 response.StatusCode = HttpStatusCode.InternalServerError;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 var err = new { error = new { message = "Internal server error", details = ex.Message } };
 await response.WriteStringAsync(JsonSerializer.Serialize(err));
 return response;
 }
 }
 }
}