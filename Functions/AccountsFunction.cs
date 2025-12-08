using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services;
using Models;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.IO;
using Helpers;

namespace Functions
{
 public class AccountsFunction
 {
 private readonly IAccountsService _service;
 private readonly ILogger<AccountsFunction> _logger;
 private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

 public AccountsFunction(IAccountsService service, ILogger<AccountsFunction> logger)
 {
 _service = service;
 _logger = logger;
 }

 [Function("GetAllAccounts")]
 public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts")] HttpRequestData req)
 {
 try
 {
 var accounts = await _service.GetAllAsync();
 var resp = req.CreateResponse(System.Net.HttpStatusCode.OK);
 await resp.WriteAsJsonAsync(accounts, _jsonOptions);
 return resp;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "Error in GetAll");
 return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.InternalServerError, "ERR_GET_ALL", ex.Message);
 }
 }

 [Function("GetAccountById")]
 public async Task<HttpResponseData> GetById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{id}")] HttpRequestData req, string id)
 {
 try
 {
 var account = await _service.GetByIdAsync(id);
 if (account == null)
 {
 return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.NotFound, "ERR_NOT_FOUND", $"Account with id {id} not found");
 }
 var resp = req.CreateResponse(System.Net.HttpStatusCode.OK);
 await resp.WriteAsJsonAsync(account, _jsonOptions);
 return resp;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "Error in GetById {Id}", id);
 return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.InternalServerError, "ERR_GET_BY_ID", ex.Message);
 }
 }

 [Function("CreateAccount")]
 public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts")] HttpRequestData req)
 {
 try
 {
 var body = await new StreamReader(req.Body).ReadToEndAsync();
 var account = JsonSerializer.Deserialize<Account>(body, _jsonOptions);
 if (account == null) return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.BadRequest, "ERR_INVALID_PAYLOAD", "Invalid account payload");
 var created = await _service.CreateAsync(account);
 var resp = req.CreateResponse(System.Net.HttpStatusCode.Created);
 await resp.WriteAsJsonAsync(created, _jsonOptions);
 return resp;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "Error in Create");
 return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.InternalServerError, "ERR_CREATE", ex.Message);
 }
 }

 [Function("CreateAccountsBatch")]
 public async Task<HttpResponseData> CreateBatch([HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/batch")] HttpRequestData req)
 {
 try
 {
 var body = await new StreamReader(req.Body).ReadToEndAsync();
 using var doc = JsonDocument.Parse(body);
 if (!doc.RootElement.TryGetProperty("accounts", out var accountsElement) || accountsElement.ValueKind != JsonValueKind.Array)
 {
 return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.BadRequest, "ERR_INVALID_PAYLOAD", "Payload must contain 'accounts' array");
 }
 var accounts = JsonSerializer.Deserialize<IEnumerable<Account>>(accountsElement.GetRawText(), _jsonOptions);
 if (accounts == null) return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.BadRequest, "ERR_INVALID_PAYLOAD", "Accounts payload invalid");
 var created = await _service.CreateBatchAsync(accounts);
 var resp = req.CreateResponse(System.Net.HttpStatusCode.Created);
 await resp.WriteAsJsonAsync(created, _jsonOptions);
 return resp;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "Error in CreateBatch");
 return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.InternalServerError, "ERR_CREATE_BATCH", ex.Message);
 }
 }

 [Function("UpdateAccount")]
 public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Function, "put", Route = "accounts/{id}")] HttpRequestData req, string id)
 {
 try
 {
 var body = await new StreamReader(req.Body).ReadToEndAsync();
 var update = JsonSerializer.Deserialize<Account>(body, _jsonOptions);
 if (update == null) return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.BadRequest, "ERR_INVALID_PAYLOAD", "Invalid update payload");
 var updated = await _service.UpdateAsync(id, update);
 if (updated == null) return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.NotFound, "ERR_NOT_FOUND", $"Account with id {id} not found");
 var resp = req.CreateResponse(System.Net.HttpStatusCode.OK);
 await resp.WriteAsJsonAsync(updated, _jsonOptions);
 return resp;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "Error in Update {Id}", id);
 return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.InternalServerError, "ERR_UPDATE", ex.Message);
 }
 }

 [Function("DeleteAccount")]
 public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "accounts/{id}")] HttpRequestData req, string id)
 {
 try
 {
 var deleted = await _service.DeleteAsync(id);
 if (!deleted) return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.NotFound, "ERR_NOT_FOUND", $"Account with id {id} not found");
 var resp = req.CreateResponse(System.Net.HttpStatusCode.OK);
 await resp.WriteAsJsonAsync(new { message = "Deleted" }, _jsonOptions);
 return resp;
 }
 catch (System.Exception ex)
 {
 _logger.LogError(ex, "Error in Delete {Id}", id);
 return ResponseHelper.CreateErrorResponse(req, System.Net.HttpStatusCode.InternalServerError, "ERR_DELETE", ex.Message);
 }
 }
 }
}
