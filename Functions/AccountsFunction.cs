using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SalesforceAccountFunctions.Services;
using SalesforceAccountFunctions.Models;
using SalesforceAccountFunctions.Helpers;

namespace SalesforceAccountFunctions.Functions
{
 public class AccountsFunction
 {
 private readonly SalesforceService _salesforce;
 private readonly ILogger<AccountsFunction> _logger;
 private readonly ILoggingService _logService;

 public AccountsFunction(SalesforceService salesforce, ILogger<AccountsFunction> logger, ILoggingService logService)
 {
 _salesforce = salesforce;
 _logger = logger;
 _logService = logService;
 }

 [Function("GetAccounts")]
 public async Task<HttpResponseData> GetAccounts([HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts")] HttpRequestData req)
 {
 try
 {
 var query = req.Url.Query; // optional custom SOQL via query string
 List<AccountModel> accounts;
 if (!string.IsNullOrEmpty(query) && query.Contains("soql="))
 {
 var soql = System.Web.HttpUtility.ParseQueryString(query).Get("soql");
 accounts = await _salesforce.QueryAccountsAsync(soql);
 }
 else
 {
 accounts = await _salesforce.QueryAccountsAsync(null);
 }

 await _logService.LogAsync("Accounts", "List", $"Returned {accounts.Count} accounts");
 return await ResponseHelper.CreateJsonResponseAsync(req, new { records = accounts });
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "GetAccounts failed");
 return await ResponseHelper.CreateErrorResponseAsync(req, ex.Message, "GetAccountsError", 500);
 }
 }

 [Function("GetAccountById")]
 public async Task<HttpResponseData> GetAccountById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "accounts/{id}")] HttpRequestData req, string id)
 {
 try
 {
 var account = await _salesforce.GetAccountAsync(id);
 if (account == null)
 {
 return await ResponseHelper.CreateErrorResponseAsync(req, "Account not found", "NotFound", 404);
 }
 await _logService.LogAsync("Accounts", "Get", id);
 return await ResponseHelper.CreateJsonResponseAsync(req, account);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "GetAccountById failed for {id}", id);
 return await ResponseHelper.CreateErrorResponseAsync(req, ex.Message, "GetAccountError", 500);
 }
 }

 [Function("CreateAccount")]
 public async Task<HttpResponseData> CreateAccount([HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts")] HttpRequestData req)
 {
 try
 {
 var body = await new StreamReader(req.Body).ReadToEndAsync();
 if (string.IsNullOrWhiteSpace(body))
 return await ResponseHelper.CreateErrorResponseAsync(req, "Empty request body", "BadRequest", 400);

 var model = JsonSerializer.Deserialize<CreateSingleRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
 if (model?.Record == null)
 return await ResponseHelper.CreateErrorResponseAsync(req, "Invalid payload", "BadRequest", 400);

 var result = await _salesforce.CreateAccountAsync(model.Record);
 await _logService.LogAsync("Accounts", "Create", JsonSerializer.Serialize(model.Record));
 return await ResponseHelper.CreateJsonResponseAsync(req, result, 201);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "CreateAccount failed");
 return await ResponseHelper.CreateErrorResponseAsync(req, ex.Message, "CreateAccountError", 500);
 }
 }

 [Function("CreateAccountsBulk")]
 public async Task<HttpResponseData> CreateAccountsBulk([HttpTrigger(AuthorizationLevel.Function, "post", Route = "accounts/batch")] HttpRequestData req)
 {
 try
 {
 var body = await new StreamReader(req.Body).ReadToEndAsync();
 if (string.IsNullOrWhiteSpace(body))
 return await ResponseHelper.CreateErrorResponseAsync(req, "Empty request body", "BadRequest", 400);

 var model = JsonSerializer.Deserialize<BulkCreateRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
 if (model?.Records == null || model.Records.Count == 0)
 return await ResponseHelper.CreateErrorResponseAsync(req, "Invalid payload - records missing", "BadRequest", 400);

 var result = await _salesforce.CreateAccountsBulkAsync(model.Records);
 await _logService.LogAsync("Accounts", "BulkCreate", $"Created {model.Records.Count} records");
 return await ResponseHelper.CreateJsonResponseAsync(req, result, 201);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "CreateAccountsBulk failed");
 return await ResponseHelper.CreateErrorResponseAsync(req, ex.Message, "CreateAccountsBulkError", 500);
 }
 }

 [Function("UpdateAccount")]
 public async Task<HttpResponseData> UpdateAccount([HttpTrigger(AuthorizationLevel.Function, "patch", "post", Route = "accounts/{id}")] HttpRequestData req, string id)
 {
 try
 {
 var body = await new StreamReader(req.Body).ReadToEndAsync();
 if (string.IsNullOrWhiteSpace(body))
 return await ResponseHelper.CreateErrorResponseAsync(req, "Empty request body", "BadRequest", 400);

 // Accept either full AccountModel or a dictionary of fields
 Dictionary<string, object?> updates;
 try
 {
 updates = JsonSerializer.Deserialize<Dictionary<string, object?>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Dictionary<string, object?>();
 }
 catch
 {
 var model = JsonSerializer.Deserialize<AccountModel>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
 if (model == null)
 return await ResponseHelper.CreateErrorResponseAsync(req, "Invalid payload", "BadRequest", 400);
 updates = new Dictionary<string, object?>()
 {
 { "Name", model.Name },
 { "Phone", model.Phone },
 { "BillingCity", model.BillingCity },
 { "Industry", model.Industry },
 { "Website", model.Website },
 { "Description", model.Description }
 };
 }

 if (updates.Count == 0)
 return await ResponseHelper.CreateErrorResponseAsync(req, "No fields to update", "BadRequest", 400);

 await _salesforce.UpdateAccountAsync(id, updates);
 await _logService.LogAsync("Accounts", "Update", $"Updated {id} with {System.Text.Json.JsonSerializer.Serialize(updates)}");
 return await ResponseHelper.CreateJsonResponseAsync(req, new { success = true });
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "UpdateAccount failed for {id}", id);
 return await ResponseHelper.CreateErrorResponseAsync(req, ex.Message, "UpdateAccountError", 500);
 }
 }

 [Function("DeleteAccount")]
 public async Task<HttpResponseData> DeleteAccount([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "accounts/{id}")] HttpRequestData req, string id)
 {
 try
 {
 await _salesforce.DeleteAccountAsync(id);
 await _logService.LogAsync("Accounts", "Delete", id);
 return await ResponseHelper.CreateJsonResponseAsync(req, new { success = true });
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "DeleteAccount failed for {id}", id);
 return await ResponseHelper.CreateErrorResponseAsync(req, ex.Message, "DeleteAccountError", 500);
 }
 }
 }
}
