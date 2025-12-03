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
 using var sr = new StreamReader(req.Body);
 var body = await sr.ReadToEndAsync();
 var account = JsonSerializer.Deserialize<Account>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
 if (account == null)
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 var err = new ErrorResponse { Code = "invalid_request", Message = "Request body must be a valid account JSON object." };
 await response.WriteAsJsonAsync(err);
 return response;
 }

 var updated = await _accountService.UpdateAccountAsync(id, account);
 if (updated == null)
 {
 response.StatusCode = HttpStatusCode.NotFound;
 var err = new ErrorResponse { Code = "not_found", Message = $"Account with id {id} not found." };
 await response.WriteAsJsonAsync(err);
 return response;
 }

 response.StatusCode = HttpStatusCode.OK;
 await response.WriteAsJsonAsync(updated);
 return response;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "Error updating account {Id}", id);
 response.StatusCode = HttpStatusCode.InternalServerError;
 var err = new ErrorResponse { Code = "internal_error", Message = "Failed to update account.", Details = ex.Message };
 await response.WriteAsJsonAsync(err);
 return response;
 }
 }
 }
}