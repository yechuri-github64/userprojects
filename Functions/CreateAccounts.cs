using System;
using System.Collections.Generic;
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
 public class CreateAccounts
 {
 private readonly IAccountService _accountService;
 private readonly ILogger<CreateAccounts> _logger;

 public CreateAccounts(IAccountService accountService, ILogger<CreateAccounts> logger)
 {
 _accountService = accountService;
 _logger = logger;
 }

 [Function("CreateAccounts")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/batch")] HttpRequestData req)
 {
 var response = req.CreateResponse();
 try
 {
 var body = await new StreamReader(req.Body).ReadToEndAsync();
 var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
 var accounts = JsonSerializer.Deserialize<List<Account>>(body, options);
 if (accounts == null || accounts.Count == 0)
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 var err = new { error = new { message = "Request body must be a non-empty array of accounts" } };
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 await response.WriteStringAsync(JsonSerializer.Serialize(err));
 return response;
 }

 var created = await _accountService.CreateAccountsAsync(accounts);
 response.StatusCode = HttpStatusCode.Created;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 await response.WriteStringAsync(JsonSerializer.Serialize(created, options));
 return response;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "CreateAccounts failed");
 response.StatusCode = HttpStatusCode.InternalServerError;
 response.Headers.Add("Content-Type", "application/json; charset=utf-8");
 var err = new { error = new { message = "Internal server error", details = ex.Message } };
 await response.WriteStringAsync(JsonSerializer.Serialize(err));
 return response;
 }
 }
 }
}