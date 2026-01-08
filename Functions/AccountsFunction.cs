using System;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using AccountManagerFunctionApp.Repositories;
using AccountManagerFunctionApp.Models;

namespace AccountManagerFunctionApp.Functions
{
 public class AccountsFunction
 {
 private readonly IAccountRepository _repo;
 private readonly ILogger<AccountsFunction> _logger;
 private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
 {
 PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
 DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
 };

 public AccountsFunction(IAccountRepository repo, ILogger<AccountsFunction> logger)
 {
 _repo = repo;
 _logger = logger;
 }

 // GET -> Implement only GET
 [Function("GetAccounts")]
 public async Task<HttpResponseData> GetAccounts(
 [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "accounts/{id?}")] HttpRequestData req,
 string id,
 FunctionContext executionContext)
 {
 try
 {
 if (!string.IsNullOrWhiteSpace(id))
 {
 if (!Guid.TryParse(id, out var guid))
 {
 var badResp = req.CreateResponse(HttpStatusCode.BadRequest);
 var err = new { error = new { code = "InvalidId", message = "Id must be a valid GUID.", details = "Provided id is not a GUID." } };
 await badResp.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return badResp;
 }

 var account = await _repo.GetByIdAsync(guid);
 if (account == null)
 {
 var notFound = req.CreateResponse(HttpStatusCode.NotFound);
 var err = new { error = new { code = "NotFound", message = "Account not found.", details = $"No account with id {id} was found." } };
 await notFound.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return notFound;
 }

 var resp = req.CreateResponse(HttpStatusCode.OK);
 await resp.WriteStringAsync(JsonSerializer.Serialize(account, JsonOptions));
 return resp;
 }

 var list = await _repo.GetAllAsync();
 var ok = req.CreateResponse(HttpStatusCode.OK);
 await ok.WriteStringAsync(JsonSerializer.Serialize(list, JsonOptions));
 return ok;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error in GetAccounts");
 var resp = req.CreateResponse(HttpStatusCode.InternalServerError);
 var err = new { error = new { code = "InternalError", message = "An unexpected error occurred.", details = ex.Message } };
 await resp.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return resp;
 }
 }

 // POST -> Implement only POST
 [Function("CreateAccounts")]
 public async Task<HttpResponseData> CreateAccounts(
 [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "accounts")] HttpRequestData req,
 FunctionContext executionContext)
 {
 try
 {
 var body = await req.ReadAsStringAsync();
 if (string.IsNullOrWhiteSpace(body))
 {
 var bad = req.CreateResponse(HttpStatusCode.BadRequest);
 var err = new { error = new { code = "EmptyBody", message = "Request body is empty.", details = "Provide a JSON array of accounts to create." } };
 await bad.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return bad;
 }

 List<Account>? incoming;
 try
 {
 // Expecting an array of accounts for batch create
 incoming = JsonSerializer.Deserialize<List<Account>>(body, JsonOptions);
 }
 catch (JsonException jex)
 {
 _logger.LogWarning(jex, "Invalid JSON for CreateAccounts");
 var bad = req.CreateResponse(HttpStatusCode.BadRequest);
 var err = new { error = new { code = "InvalidJson", message = "Unable to parse JSON body.", details = jex.Message } };
 await bad.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return bad;
 }

 if (incoming == null || incoming.Count == 0)
 {
 var bad = req.CreateResponse(HttpStatusCode.BadRequest);
 var err = new { error = new { code = "InvalidInput", message = "Provide a non-empty JSON array of accounts.", details = "Array must contain at least one account object." } };
 await bad.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return bad;
 }

 var created = await _repo.CreateBatchAsync(incoming);
 var resp = req.CreateResponse(HttpStatusCode.Created);
 await resp.WriteStringAsync(JsonSerializer.Serialize(created, JsonOptions));
 return resp;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error in CreateAccounts");
 var resp = req.CreateResponse(HttpStatusCode.InternalServerError);
 var err = new { error = new { code = "InternalError", message = "An unexpected error occurred.", details = ex.Message } };
 await resp.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return resp;
 }
 }

 // PUT -> Implement only PUT (single update)
 [Function("UpdateAccount")]
 public async Task<HttpResponseData> UpdateAccount(
 [HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "accounts/{id}")] HttpRequestData req,
 string id,
 FunctionContext executionContext)
 {
 try
 {
 if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var guid))
 {
 var bad = req.CreateResponse(HttpStatusCode.BadRequest);
 var err = new { error = new { code = "InvalidId", message = "Id must be a valid GUID in the route.", details = "Provide a valid GUID id in route parameter." } };
 await bad.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return bad;
 }

 var body = await req.ReadAsStringAsync();
 if (string.IsNullOrWhiteSpace(body))
 {
 var bad = req.CreateResponse(HttpStatusCode.BadRequest);
 var err = new { error = new { code = "EmptyBody", message = "Request body is empty.", details = "Provide JSON with fields to update (name, email, address)." } };
 await bad.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return bad;
 }

 Account? updated;
 try
 {
 updated = JsonSerializer.Deserialize<Account>(body, JsonOptions);
 }
 catch (JsonException jex)
 {
 _logger.LogWarning(jex, "Invalid JSON for UpdateAccount");
 var bad = req.CreateResponse(HttpStatusCode.BadRequest);
 var err = new { error = new { code = "InvalidJson", message = "Unable to parse JSON body.", details = jex.Message } };
 await bad.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return bad;
 }

 if (updated == null)
 {
 var bad = req.CreateResponse(HttpStatusCode.BadRequest);
 var err = new { error = new { code = "InvalidInput", message = "Invalid account data.", details = "Provide a JSON object with fields 'name', 'email' or 'address' to update." } };
 await bad.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return bad;
 }

 var result = await _repo.UpdateAsync(guid, updated);
 if (result == null)
 {
 var notFound = req.CreateResponse(HttpStatusCode.NotFound);
 var err = new { error = new { code = "NotFound", message = "Account not found.", details = $"No account with id {id} was found." } };
 await notFound.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return notFound;
 }

 var resp = req.CreateResponse(HttpStatusCode.OK);
 await resp.WriteStringAsync(JsonSerializer.Serialize(result, JsonOptions));
 return resp;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error in UpdateAccount");
 var resp = req.CreateResponse(HttpStatusCode.InternalServerError);
 var err = new { error = new { code = "InternalError", message = "An unexpected error occurred.", details = ex.Message } };
 await resp.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return resp;
 }
 }

 // DELETE -> Implement only DELETE
 [Function("DeleteAccount")]
 public async Task<HttpResponseData> DeleteAccount(
 [HttpTrigger(AuthorizationLevel.Function, "DELETE", Route = "accounts/{id}")] HttpRequestData req,
 string id,
 FunctionContext executionContext)
 {
 try
 {
 if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var guid))
 {
 var bad = req.CreateResponse(HttpStatusCode.BadRequest);
 var err = new { error = new { code = "InvalidId", message = "Id must be a valid GUID in the route.", details = "Provide a valid GUID id in route parameter." } };
 await bad.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return bad;
 }

 var deleted = await _repo.DeleteAsync(guid);
 if (!deleted)
 {
 var notFound = req.CreateResponse(HttpStatusCode.NotFound);
 var err = new { error = new { code = "NotFound", message = "Account not found.", details = $"No account with id {id} was found." } };
 await notFound.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return notFound;
 }

 var resp = req.CreateResponse(HttpStatusCode.NoContent);
 return resp;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error in DeleteAccount");
 var resp = req.CreateResponse(HttpStatusCode.InternalServerError);
 var err = new { error = new { code = "InternalError", message = "An unexpected error occurred.", details = ex.Message } };
 await resp.WriteStringAsync(JsonSerializer.Serialize(err, JsonOptions));
 return resp;
 }
 }
 }
}
