using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using acc-sf-test.Models;

namespace acc-sf-test.Repositories
{
 public class SalesforceRepository : ISalesforceRepository
 {
 private readonly IHttpClientFactory _factory;
 private readonly SalesforceSettings _settings;
 private readonly ILogger<SalesforceRepository> _logger;
 private HttpClient? _client;
 private string _apiVersion = "59.0";

 public SalesforceRepository(IHttpClientFactory factory, SalesforceSettings settings, ILogger<SalesforceRepository> logger)
 {
 _factory = factory;
 _settings = settings;
 _logger = logger;
 }

 public Task ConfigureAsync(string? instanceUrl, string? accessToken, string apiVersion)
 {
 _apiVersion = string.IsNullOrWhiteSpace(apiVersion) ? _apiVersion : apiVersion;

 // Create client with base address set to instanceUrl. Use fallback if missing
 if (string.IsNullOrWhiteSpace(instanceUrl))
 {
 _logger.LogWarning("InstanceUrl is empty; using AuthUrl as fallback for base address.");
 instanceUrl = _settings.InstanceUrl;
 }

 if (!string.IsNullOrWhiteSpace(instanceUrl) && Uri.TryCreate(instanceUrl, UriKind.Absolute, out var baseUri))
 {
 _client = _factory.CreateClient();
 _client.BaseAddress = new Uri(baseUri, $"/services/data/v{_apiVersion}/");
 _client.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds > 0 ? _settings.TimeoutSeconds : 120);
 if (!string.IsNullOrWhiteSpace(accessToken))
 {
 _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
 }
 }
 else
 {
 _logger.LogError("Cannot configure HttpClient because instanceUrl is invalid: {Instance}", instanceUrl);
 throw new ArgumentException("Invalid instance URL for Salesforce client.");
 }

 return Task.CompletedTask;
 }

 public async Task<IEnumerable<object>> CreateAccountsAsync(IEnumerable<AccountDto> accounts)
 {
 if (_client == null) throw new InvalidOperationException("Salesforce client not configured.");
 var results = new List<object>();
 foreach (var account in accounts)
 {
 try
 {
 var json = JsonSerializer.Serialize(account);
 var resp = await _client.PostAsync("sobjects/Account/", new StringContent(json, Encoding.UTF8, "application/json"));
 var body = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Failed to create account. Status: {Status}, Body: {Body}", resp.StatusCode, body);
 results.Add(new { success = false, status = resp.StatusCode.ToString(), body });
 continue;
 }
 using var doc = JsonDocument.Parse(body);
 results.Add(new { success = true, id = doc.RootElement.GetProperty("id").GetString() });
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Exception creating account in Salesforce.");
 results.Add(new { success = false, error = ex.Message });
 }
 }
 return results;
 }

 public async Task<object?> GetAccountAsync(string id)
 {
 if (_client == null) throw new InvalidOperationException("Salesforce client not configured.");
 var path = $"sobjects/Account/{Uri.EscapeDataString(id)}";
 var resp = await _client.GetAsync(path);
 var body = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Failed to get account {Id}. Status: {Status}, Body: {Body}", id, resp.StatusCode, body);
 return new { success = false, status = resp.StatusCode.ToString(), body };
 }
 var doc = JsonDocument.Parse(body);
 return JsonSerializer.Deserialize<object>(body);
 }

 public async Task<object> UpdateAccountAsync(string id, AccountDto account)
 {
 if (_client == null) throw new InvalidOperationException("Salesforce client not configured.");
 var json = JsonSerializer.Serialize(account);
 var path = $"sobjects/Account/{Uri.EscapeDataString(id)}";
 var req = new HttpRequestMessage(new HttpMethod("PATCH"), path)
 {
 Content = new StringContent(json, Encoding.UTF8, "application/json")
 };
 var resp = await _client.SendAsync(req);
 var body = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Failed to update account {Id}. Status: {Status}, Body: {Body}", id, resp.StatusCode, body);
 return new { success = false, status = resp.StatusCode.ToString(), body };
 }
 return new { success = true };
 }

 public async Task DeleteAccountAsync(string id)
 {
 if (_client == null) throw new InvalidOperationException("Salesforce client not configured.");
 var path = $"sobjects/Account/{Uri.EscapeDataString(id)}";
 var resp = await _client.DeleteAsync(path);
 var body = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Failed to delete account {Id}. Status: {Status}, Body: {Body}", id, resp.StatusCode, body);
 throw new InvalidOperationException($"Failed to delete account: {body}");
 }
 }
 }
}
