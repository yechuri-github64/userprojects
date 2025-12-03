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
 using var sr = new StreamReader(req.Body);
 var body = await sr.ReadToEndAsync();
 var accounts = JsonSerializer.Deserialize<List<Account>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
 if (accounts == null || accounts.Count == 0)
 {
 response.StatusCode = HttpStatusCode.BadRequest;
 var err = new ErrorResponse { Code = "invalid_request", Message = "Request body must be a non-empty JSON array of accounts." };
 await response.WriteAsJsonAsync(err);
 return response;
 }

 var created = await _accountService.CreateAccountsAsync(accounts);
 response.StatusCode = HttpStatusCode.Created;
 await response.WriteAsJsonAsync(created);
 return response;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error creating accounts batch");
 response.StatusCode = HttpStatusCode.InternalServerError;
 var err = new ErrorResponse { Code = "internal_error", Message = "Failed to create accounts.", Details = ex.Message };
 await response.WriteAsJsonAsync(err);
 return response;
 }
 }
 }
}