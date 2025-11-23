using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using demo_test_accounts_salesforce_app.Models;

namespace demo_test_accounts_salesforce_app.Services
{
 public class SalesforceService : ISalesforceService
 {
 private readonly IHttpClientFactory _httpClientFactory;
 private readonly ILogger<SalesforceService> _logger;
 private readonly SalesforceSettings _settings;

 private string _accessToken;
 private string _instanceUrl;
 private DateTime _tokenExpiresAt = DateTime.MinValue;
 private readonly object _authLock = new object();

 public SalesforceService(IHttpClientFactory httpClientFactory, IOptions<SalesforceSettings> options, ILogger<SalesforceService> logger)
 {
 _httpClientFactory = httpClientFactory;
 _settings = options.Value;
 _logger = logger;
 // If appsettings contains InstanceUrl and ApiVersion, use them; otherwise will be set after auth
 _instanceUrl = _settings.InstanceUrl;
 }

 private async Task EnsureAuthenticatedAsync()
 {
 if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiresAt)
 return;

 lock (_authLock)
 {
 if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiresAt)
 return;
 }

 try
 {
 var loginUrl = _settings.UseSandbox ? _settings.SandboxLoginUrl : _settings.ProductionLoginUrl;
 if (string.IsNullOrEmpty(loginUrl)) throw new InvalidOperationException("Login URL is not configured.");

 var client = _httpClientFactory.CreateClient();
 client.BaseAddress = new Uri(loginUrl);

 var passwordWithToken = (_settings.Password ?? string.Empty) + (_settings.SecurityToken ?? string.Empty);
 var contentDict = new Dictionary<string, string>
 {
 {"grant_type", "password"},
 {"client_id", _settings.ClientId ?? string.Empty},
 {"client_secret", _settings.ClientSecret ?? string.Empty},
 {"username", _settings.Username ?? string.Empty},
 {"password", passwordWithToken}
 };

 var response = client.PostAsync("/services/oauth2/token", new FormUrlEncodedContent(contentDict)).GetAwaiter().GetResult();
 if (!response.IsSuccessStatusCode)
 {
 var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
 _logger.LogError("Failed to authenticate to Salesforce. Status: {Status}, Body: {Body}", response.StatusCode, body);
 throw new InvalidOperationException("Authentication to Salesforce failed.");
 }

 var respJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
 using var doc = JsonDocument.Parse(respJson);
 var root = doc.RootElement;
 if (root.TryGetProperty("access_token", out var at))
 {
 _accessToken = at.GetString();
 }
 if (root.TryGetProperty("instance_url", out var iu))
 {
 _instanceUrl = iu.GetString();
 }

 // set token expiry to 55 minutes from now as a safe default
 _tokenExpiresAt = DateTime.UtcNow.AddMinutes(55);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error during Salesforce authentication");
 throw;
 }
 }

 private HttpClient CreateSalesforceClient()
 {
 var client = _httpClientFactory.CreateClient();
 if (string.IsNullOrEmpty(_instanceUrl)) throw new InvalidOperationException("InstanceUrl is not available.");
 client.BaseAddress = new Uri(_instanceUrl);
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
 client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 return client;
 }

 public async Task<Account> GetAccountAsync(string id)
 {
 await EnsureAuthenticatedAsync();
 try
 {
 var client = CreateSalesforceClient();
 var apiVersion = string.IsNullOrEmpty(_settings.ApiVersion) ? "v58.0" : _settings.ApiVersion;
 var url = $"/services/data/{apiVersion}/sobjects/Account/{id}";
 var resp = await client.GetAsync(url);
 if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
 return null;
 resp.EnsureSuccessStatusCode();
 var json = await resp.Content.ReadAsStringAsync();
 var account = JsonSerializer.Deserialize<Account>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
 return account;
 }
 catch (HttpRequestException ex)
 {
 _logger.LogError(ex, "HTTP error when getting account {AccountId}", id);
 throw;
 }
 }

 public async Task<Account> CreateAccountAsync(Account account)
 {
 await EnsureAuthenticatedAsync();
 try
 {
 var client = CreateSalesforceClient();
 var apiVersion = string.IsNullOrEmpty(_settings.ApiVersion) ? "v58.0" : _settings.ApiVersion;
 var url = $"/services/data/{apiVersion}/sobjects/Account/";
 // Salesforce create returns { id: "<id>", success: true, errors: [] }
 var payload = JsonSerializer.Serialize(account, new JsonSerializerOptions { IgnoreNullValues = true });
 var resp = await client.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
 resp.EnsureSuccessStatusCode();
 var body = await resp.Content.ReadAsStringAsync();
 using var doc = JsonDocument.Parse(body);
 var root = doc.RootElement;
 if (root.TryGetProperty("id", out var idEl))
 {
 var id = idEl.GetString();
 // fetch created record to return full representation
 return await GetAccountAsync(id);
 }
 throw new InvalidOperationException("Salesforce did not return an Id on create.");
 }
 catch (HttpRequestException ex)
 {
 _logger.LogError(ex, "HTTP error when creating account");
 throw;
 }
 }

 public async Task<bool> UpdateAccountAsync(string id, Account account)
 {
 await EnsureAuthenticatedAsync();
 try
 {
 var client = CreateSalesforceClient();
 var apiVersion = string.IsNullOrEmpty(_settings.ApiVersion) ? "v58.0" : _settings.ApiVersion;
 var url = $"/services/data/{apiVersion}/sobjects/Account/{id}";
 var payload = JsonSerializer.Serialize(account, new JsonSerializerOptions { IgnoreNullValues = true });
 var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
 {
 Content = new StringContent(payload, Encoding.UTF8, "application/json")
 };
 var resp = await client.SendAsync(request);
 if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
 return false;
 resp.EnsureSuccessStatusCode();
 return true;
 }
 catch (HttpRequestException ex)
 {
 _logger.LogError(ex, "HTTP error when updating account {AccountId}", id);
 throw;
 }
 }

 public async Task<bool> DeleteAccountAsync(string id)
 {
 await EnsureAuthenticatedAsync();
 try
 {
 var client = CreateSalesforceClient();
 var apiVersion = string.IsNullOrEmpty(_settings.ApiVersion) ? "v58.0" : _settings.ApiVersion;
 var url = $"/services/data/{apiVersion}/sobjects/Account/{id}";
 var resp = await client.DeleteAsync(url);
 if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
 return false;
 resp.EnsureSuccessStatusCode();
 return true;
 }
 catch (HttpRequestException ex)
 {
 _logger.LogError(ex, "HTTP error when deleting account {AccountId}", id);
 throw;
 }
 }
 }
}
