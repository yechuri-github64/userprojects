using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using acc_sf_test.Models;
using Microsoft.Extensions.Options;

namespace acc_sf_test.Services
{
 public class SalesforceService : ISalesforceService
 {
 private readonly IHttpClientFactory _httpFactory;
 private readonly SalesforceOptions _options;
 private readonly ILogger<SalesforceService> _logger;

 public SalesforceService(IHttpClientFactory httpFactory, IOptions<SalesforceOptions> options, ILogger<SalesforceService> logger)
 {
 _httpFactory = httpFactory;
 _options = options.Value;
 _logger = logger;
 }

 public async Task AuthenticateAsync(CancellationToken ct = default)
 {
 // If we already have a token, don't reauthenticate here; caller may control caching
 _logger.LogInformation("Starting Salesforce authentication");

 if (string.IsNullOrWhiteSpace(_options.ClientId) || string.IsNullOrWhiteSpace(_options.ClientSecret) || string.IsNullOrWhiteSpace(_options.Username) || string.IsNullOrWhiteSpace(_options.Password))
 {
 _logger.LogError("Missing Salesforce credentials in configuration (ClientId/ClientSecret/Username/Password). Authentication cannot proceed.");
 throw new ApplicationException("Missing Salesforce credentials in configuration");
 }

 if (!Uri.TryCreate(_options.TokenUrl, UriKind.Absolute, out var tokenUri))
 {
 _logger.LogError("Invalid TokenUrl configured: {TokenUrl}", _options.TokenUrl);
 throw new ApplicationException("Invalid Salesforce TokenUrl");
 }

 var client = _httpFactory.CreateClient();
 client.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);

 // Build password+security token as required by SF when using username/password flow
 var passwordWithToken = _options.Password + (_options.SecurityToken ?? string.Empty);

 var form = new Dictionary<string, string>
 {
 { "grant_type", "password" },
 { "client_id", _options.ClientId },
 { "client_secret", _options.ClientSecret },
 { "username", _options.Username },
 { "password", passwordWithToken }
 };

 var content = new FormUrlEncodedContent(form);

 for (int attempt = 0; attempt <= _options.RetryPolicy.MaxRetries; attempt++)
 {
 try
 {
 var resp = await client.PostAsync(tokenUri, content, ct);
 var respText = await resp.Content.ReadAsStringAsync(ct);
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogWarning("Salesforce token request failed (attempt {Attempt}/{Max}). Status: {Status}. Response: {Response}", attempt + 1, _options.RetryPolicy.MaxRetries + 1, resp.StatusCode, respText);
 if (attempt == _options.RetryPolicy.MaxRetries)
 {
 throw new ApplicationException($"Salesforce token request failed: {resp.StatusCode} - {respText}");
 }
 await Task.Delay(TimeSpan.FromSeconds(_options.RetryPolicy.DelaySeconds), ct);
 continue;
 }

 using var doc = JsonDocument.Parse(respText);
 if (doc.RootElement.TryGetProperty("access_token", out var tkn))
 {
 _options.AccessToken = tkn.GetString();
 if (doc.RootElement.TryGetProperty("instance_url", out var inst))
 {
 var instanceUrl = inst.GetString();
 if (!string.IsNullOrWhiteSpace(instanceUrl))
 {
 _options.InstanceUrl = instanceUrl;
 _logger.LogInformation("Salesforce authenticated successfully. InstanceUrl set to {InstanceUrl}", _options.InstanceUrl);
 }
 else
 {
 _logger.LogWarning("Authentication returned empty instance_url; using configured InstanceUrl: {InstanceUrl}", _options.InstanceUrl);
 }
 }
 return;
 }

 _logger.LogError("Token response did not contain access_token. Response: {Response}", respText);
 throw new ApplicationException("Salesforce token response did not contain access_token");
 }
 catch (Exception ex) when (attempt < _options.RetryPolicy.MaxRetries)
 {
 _logger.LogWarning(ex, "Exception on Salesforce token request attempt {Attempt}. Retrying after {Delay}s", attempt + 1, _options.RetryPolicy.DelaySeconds);
 await Task.Delay(TimeSpan.FromSeconds(_options.RetryPolicy.DelaySeconds), ct);
 }
 }

 _logger.LogError("Exhausted retries while trying to get Salesforce token");
 throw new ApplicationException("Unable to authenticate to Salesforce after retries");
 }

 private HttpClient CreateApiClient()
 {
 if (!Uri.TryCreate(_options.InstanceUrl, UriKind.Absolute, out var baseUri))
 {
 _logger.LogError("Invalid InstanceUrl configured: {InstanceUrl}", _options.InstanceUrl);
 throw new ApplicationException("Invalid Salesforce InstanceUrl");
 }

 var client = _httpFactory.CreateClient();
 client.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
 client.BaseAddress = baseUri;
 if (!string.IsNullOrWhiteSpace(_options.AccessToken))
 {
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
 }
 return client;
 }

 public async Task<HttpResponseMessage> SendApiAsync(HttpMethod method, string path, HttpContent? content = null, CancellationToken ct = default)
 {
 if (string.IsNullOrWhiteSpace(_options.AccessToken))
 {
 _logger.LogInformation("No access token present. Authenticating before API call...");
 await AuthenticateAsync(ct);
 }

 var client = CreateApiClient();
 var request = new HttpRequestMessage(method, path) { Content = content };

 for (int attempt = 0; attempt <= _options.RetryPolicy.MaxRetries; attempt++)
 {
 try
 {
 var resp = await client.SendAsync(request, ct);
 if ((int)resp.StatusCode >= 500 && attempt < _options.RetryPolicy.MaxRetries)
 {
 _logger.LogWarning("Salesforce API returned server error {Status}. Attempt {Attempt}/{Max}", resp.StatusCode, attempt + 1, _options.RetryPolicy.MaxRetries + 1);
 await Task.Delay(TimeSpan.FromSeconds(_options.RetryPolicy.DelaySeconds), ct);
 continue;
 }
 return resp;
 }
 catch (Exception ex) when (attempt < _options.RetryPolicy.MaxRetries)
 {
 _logger.LogWarning(ex, "Exception during Salesforce API call attempt {Attempt}. Retrying after {Delay}s", attempt + 1, _options.RetryPolicy.DelaySeconds);
 await Task.Delay(TimeSpan.FromSeconds(_options.RetryPolicy.DelaySeconds), ct);
 }
 }

 _logger.LogError("Failed to call Salesforce API after retries");
 throw new ApplicationException("Failed to call Salesforce API");
 }
 }
}