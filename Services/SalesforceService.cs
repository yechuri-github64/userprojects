using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using SalesforceAccountFunctions.Models;
using SalesforceAccountFunctions.Helpers;

namespace SalesforceAccountFunctions.Services
{
 public class SalesforceOptions
 {
 public string ClientId { get; set; } = string.Empty;
 public string ClientSecret { get; set; } = string.Empty;
 public string Username { get; set; } = string.Empty;
 public string Password { get; set; } = string.Empty; // include security token if required
 public string TokenUrl { get; set; } = "https://login.salesforce.com/services/oauth2/token";
 public string ApiVersion { get; set; } = "v56.0";
 }

 public class SalesforceService
 {
 private readonly IHttpClientFactory _httpClientFactory;
 private readonly SalesforceOptions _options;
 private readonly ILogger<SalesforceService> _logger;
 private readonly ILoggingService _logService;

 private string? _accessToken;
 private string? _instanceUrl;
 private DateTime _tokenExpiry = DateTime.MinValue;

 public SalesforceService(IHttpClientFactory httpClientFactory, IOptions<SalesforceOptions> options, ILogger<SalesforceService> logger, ILoggingService logService)
 {
 _httpClientFactory = httpClientFactory;
 _options = options.Value;
 _logger = logger;
 _logService = logService;
 }

 private async Task AuthenticateAsync()
 {
 if (!string.IsNullOrEmpty(_accessToken) && _tokenExpiry > DateTime.UtcNow.AddMinutes(1))
 return;

 var client = _httpClientFactory.CreateClient();
 var content = new FormUrlEncodedContent(new[]
 {
 new KeyValuePair<string, string>("grant_type", "password"),
 new KeyValuePair<string, string>("client_id", _options.ClientId),
 new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
 new KeyValuePair<string, string>("username", _options.Username),
 new KeyValuePair<string, string>("password", _options.Password)
 });

 var resp = await client.PostAsync(_options.TokenUrl, content);
 var respString = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Salesforce authentication failed: {status} {body}", resp.StatusCode, respString);
 await _logService.LogErrorAsync("SalesforceAuth", resp.StatusCode.ToString(), respString);
 throw new ApplicationException($"Salesforce authentication failed: {respString}");
 }

 using var doc = JsonDocument.Parse(respString);
 _accessToken = doc.RootElement.GetProperty("access_token").GetString();
 _instanceUrl = doc.RootElement.GetProperty("instance_url").GetString();
 // Salesforce token responses typically don't give expires_in for password flow; set a short expiry
 _tokenExpiry = DateTime.UtcNow.AddMinutes(55);
 }

 private HttpRequestMessage CreateRequest(HttpMethod method, string path, object? body = null)
 {
 var req = new HttpRequestMessage(method, path);
 req.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
 if (body != null)
 {
 var json = JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
 req.Content = new StringContent(json, Encoding.UTF8, "application/json");
 }
 return req;
 }

 public async Task<List<AccountModel>> QueryAccountsAsync(string? soql = null)
 {
 try
 {
 await AuthenticateAsync();
 var client = _httpClientFactory.CreateClient();
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

 var query = soql ?? "SELECT Id, Name, Phone, BillingCity, Industry, Website, Description FROM Account LIMIT 200";
 var url = $"{_instanceUrl}/services/data/{_options.ApiVersion}/query?q={System.Net.WebUtility.UrlEncode(query)}";
 var resp = await client.GetAsync(url);
 var body = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Salesforce query failed: {status} {body}", resp.StatusCode, body);
 await _logService.LogErrorAsync("QueryAccounts", resp.StatusCode.ToString(), body);
 throw new ApplicationException(body);
 }

 using var doc = JsonDocument.Parse(body);
 var records = new List<AccountModel>();
 if (doc.RootElement.TryGetProperty("records", out var recs))
 {
 foreach (var r in recs.EnumerateArray())
 {
 var account = JsonSerializer.Deserialize<AccountModel>(r.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
 if (account != null) records.Add(account);
 }
 }
 return records;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error querying accounts");
 await _logService.LogErrorAsync("QueryAccounts_Exception", "Exception", ex.ToString());
 throw;
 }
 }

 public async Task<AccountModel?> GetAccountAsync(string id)
 {
 try
 {
 await AuthenticateAsync();
 var client = _httpClientFactory.CreateClient();
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
 var url = $"{_instanceUrl}/services/data/{_options.ApiVersion}/sobjects/Account/{id}";
 var resp = await client.GetAsync(url);
 var body = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Salesforce get failed: {status} {body}", resp.StatusCode, body);
 await _logService.LogErrorAsync("GetAccount", resp.StatusCode.ToString(), body);
 throw new ApplicationException(body);
 }
 var account = JsonSerializer.Deserialize<AccountModel>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
 return account;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error getting account {id}", id);
 await _logService.LogErrorAsync("GetAccount_Exception", id, ex.ToString());
 throw;
 }
 }

 public async Task<Dictionary<string, object>> CreateAccountAsync(AccountModel account)
 {
 try
 {
 await AuthenticateAsync();
 var client = _httpClientFactory.CreateClient();
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
 var url = $"{_instanceUrl}/services/data/{_options.ApiVersion}/sobjects/Account/";
 var body = new Dictionary<string, object?>()
 {
 { "Name", account.Name },
 { "Phone", account.Phone },
 { "BillingCity", account.BillingCity },
 { "Industry", account.Industry },
 { "Website", account.Website },
 { "Description", account.Description }
 };
 var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
 var resp = await client.PostAsync(url, content);
 var respBody = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Salesforce create failed: {status} {body}", resp.StatusCode, respBody);
 await _logService.LogErrorAsync("CreateAccount", resp.StatusCode.ToString(), respBody);
 throw new ApplicationException(respBody);
 }
 var result = JsonSerializer.Deserialize<Dictionary<string, object>>(respBody);
 return result ?? new Dictionary<string, object>();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error creating account");
 await _logService.LogErrorAsync("CreateAccount_Exception", "Exception", ex.ToString());
 throw;
 }
 }

 public async Task<Dictionary<string, object>> CreateAccountsBulkAsync(IEnumerable<AccountModel> accounts)
 {
 try
 {
 await AuthenticateAsync();
 var client = _httpClientFactory.CreateClient();
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
 var url = $"{_instanceUrl}/services/data/{_options.ApiVersion}/composite/sobjects";

 var records = accounts.Select(a => new Dictionary<string, object?>()
 {
 { "attributes", new { type = "Account" } },
 { "Name", a.Name },
 { "Phone", a.Phone },
 { "BillingCity", a.BillingCity },
 { "Industry", a.Industry },
 { "Website", a.Website },
 { "Description", a.Description }
 }).ToList();

 var payload = new { allOrNone = false, records };
 var content = new StringContent(JsonSerializer.Serialize(payload, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }), Encoding.UTF8, "application/json");
 var resp = await client.PostAsync(url, content);
 var respBody = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Salesforce bulk create failed: {status} {body}", resp.StatusCode, respBody);
 await _logService.LogErrorAsync("CreateAccountsBulk", resp.StatusCode.ToString(), respBody);
 throw new ApplicationException(respBody);
 }
 var result = JsonSerializer.Deserialize<Dictionary<string, object>>(respBody);
 return result ?? new Dictionary<string, object>();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error creating accounts bulk");
 await _logService.LogErrorAsync("CreateAccountsBulk_Exception", "Exception", ex.ToString());
 throw;
 }
 }

 public async Task UpdateAccountAsync(string id, Dictionary<string, object?> updates)
 {
 try
 {
 await AuthenticateAsync();
 var client = _httpClientFactory.CreateClient();
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
 var url = $"{_instanceUrl}/services/data/{_options.ApiVersion}/sobjects/Account/{id}";

 var content = new StringContent(JsonSerializer.Serialize(updates, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }), Encoding.UTF8, "application/json");
 var request = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = content };
 var resp = await client.SendAsync(request);
 var respBody = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Salesforce update failed: {status} {body}", resp.StatusCode, respBody);
 await _logService.LogErrorAsync("UpdateAccount", resp.StatusCode.ToString(), respBody);
 throw new ApplicationException(respBody);
 }
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error updating account {id}", id);
 await _logService.LogErrorAsync("UpdateAccount_Exception", id, ex.ToString());
 throw;
 }
 }

 public async Task DeleteAccountAsync(string id)
 {
 try
 {
 await AuthenticateAsync();
 var client = _httpClientFactory.CreateClient();
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
 var url = $"{_instanceUrl}/services/data/{_options.ApiVersion}/sobjects/Account/{id}";
 var resp = await client.DeleteAsync(url);
 var respBody = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Salesforce delete failed: {status} {body}", resp.StatusCode, respBody);
 await _logService.LogErrorAsync("DeleteAccount", resp.StatusCode.ToString(), respBody);
 throw new ApplicationException(respBody);
 }
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error deleting account {id}", id);
 await _logService.LogErrorAsync("DeleteAccount_Exception", id, ex.ToString());
 throw;
 }
 }
 }
}
