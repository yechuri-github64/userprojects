using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using acc-sf-test.Models;
using acc-sf-test.Repositories;

namespace acc-sf-test.Services
{
 public class SalesforceService : ISalesforceService
 {
 private readonly SalesforceSettings _settings;
 private readonly IHttpClientFactory _httpClientFactory;
 private readonly ISalesforceRepository _repository;
 private readonly ILogger<SalesforceService> _logger;
 private string? _accessToken;
 private string? _instanceUrl;

 public SalesforceService(SalesforceSettings settings, IHttpClientFactory httpClientFactory, ISalesforceRepository repository, ILogger<SalesforceService> logger)
 {
 _settings = settings;
 _httpClientFactory = httpClientFactory;
 _repository = repository;
 _logger = logger;
 }

 public async Task AuthenticateAsync()
 {
 _logger.LogInformation("Starting Salesforce authentication.");

 if (string.IsNullOrWhiteSpace(_settings.TokenUrl) || !Uri.TryCreate(_settings.TokenUrl, UriKind.Absolute, out var tokenUri))
 {
 _logger.LogError("Invalid TokenUrl: {TokenUrl}", _settings.TokenUrl);
 throw new ArgumentException("Invalid Salesforce TokenUrl configuration.");
 }

 var client = _httpClientFactory.CreateClient();
 client.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds > 0 ? _settings.TimeoutSeconds : 120);

 var passwordWithToken = _settings.Password ?? string.Empty;
 if (!string.IsNullOrEmpty(_settings.SecurityToken)) passwordWithToken += _settings.SecurityToken;

 var content = new FormUrlEncodedContent(new Dictionary<string, string>
 {
 { "grant_type", "password" },
 { "client_id", _settings.ClientId ?? string.Empty },
 { "client_secret", _settings.ClientSecret ?? string.Empty },
 { "username", _settings.Username ?? string.Empty },
 { "password", passwordWithToken }
 });

 HttpResponseMessage resp;
 try
 {
 resp = await client.PostAsync(tokenUri, content);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error calling Salesforce token endpoint {Url}", _settings.TokenUrl);
 throw;
 }

 var body = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogError("Salesforce token endpoint returned {Status}: {Body}", resp.StatusCode, body);
 throw new InvalidOperationException("Failed to retrieve Salesforce access token. See logs for details.");
 }

 using var doc = JsonDocument.Parse(body);
 if (doc.RootElement.TryGetProperty("access_token", out var tokenEl))
 {
 _accessToken = tokenEl.GetString();
 _logger.LogInformation("Salesforce authentication succeeded.");
 }
 if (doc.RootElement.TryGetProperty("instance_url", out var instanceEl))
 {
 _instanceUrl = instanceEl.GetString();
 _logger.LogInformation("Salesforce instance URL: {Instance}", _instanceUrl);
 }

 // Ensure repository has client configured
 await _repository.ConfigureAsync(_instanceUrl, _accessToken, _settings.ApiVersion);
 }

 public async Task<IEnumerable<object>> CreateAccountsAsync(IEnumerable<AccountDto> accounts)
 {
 if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_instanceUrl))
 {
 _logger.LogInformation("No access token or instance URL present. Authenticating...");
 await AuthenticateAsync();
 }

 return await _repository.CreateAccountsAsync(accounts);
 }

 public async Task<object?> GetAccountAsync(string id)
 {
 if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_instanceUrl))
 {
 _logger.LogInformation("No access token or instance URL present. Authenticating...");
 await AuthenticateAsync();
 }

 return await _repository.GetAccountAsync(id);
 }

 public async Task<object> UpdateAccountAsync(string id, AccountDto account)
 {
 if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_instanceUrl))
 {
 _logger.LogInformation("No access token or instance URL present. Authenticating...");
 await AuthenticateAsync();
 }

 return await _repository.UpdateAccountAsync(id, account);
 }

 public async Task DeleteAccountAsync(string id)
 {
 if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_instanceUrl))
 {
 _logger.LogInformation("No access token or instance URL present. Authenticating...");
 await AuthenticateAsync();
 }

 await _repository.DeleteAccountAsync(id);
 }
 }
}
