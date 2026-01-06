using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace accounts_management_c_sharp.Services
{
 public class SalesforceClient : ISalesforceClient
 {
 private readonly IHttpClientFactory _httpClientFactory;
 private readonly IConfiguration _configuration;
 private readonly ILogger<SalesforceClient> _logger;
 private string _accessToken;
 private string _instanceUrl;

 public SalesforceClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<SalesforceClient> logger)
 {
 _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
 _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
 _logger = logger ?? throw new ArgumentNullException(nameof(logger));
 }

 public async Task EnsureAuthenticatedAsync()
 {
 if (!string.IsNullOrEmpty(_accessToken) && !string.IsNullOrEmpty(_instanceUrl)) return;

 var useSandbox = _configuration.GetValue<bool>("Salesforce:UseSandbox");
 var loginUrl = useSandbox ? _configuration.GetValue<string>("Salesforce:LoginUrlSandbox") : _configuration.GetValue<string>("Salesforce:LoginUrlProduction");
 var clientId = _configuration.GetValue<string>("Salesforce:ClientId");
 var clientSecret = _configuration.GetValue<string>("Salesforce:ClientSecret");
 var username = _configuration.GetValue<string>("Salesforce:Username");
 var password = _configuration.GetValue<string>("Salesforce:Password");
 var securityToken = _configuration.GetValue<string>("Salesforce:SecurityToken");

 var http = _httpClientFactory.CreateClient();
 var body = new StringContent($"grant_type=password&client_id={Uri.EscapeDataString(clientId)}&client_secret={Uri.EscapeDataString(clientSecret)}&username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password + securityToken)}", Encoding.UTF8, "application/x-www-form-urlencoded");

 var resp = await http.PostAsync(loginUrl.TrimEnd('/') + "/services/oauth2/token", body);
 if (!resp.IsSuccessStatusCode)
 {
 var err = await resp.Content.ReadAsStringAsync();
 _logger.LogError("Salesforce authentication failed: {Status} {Body}", resp.StatusCode, err);
 throw new Exception("Failed to authenticate to Salesforce");
 }

 var payload = await resp.Content.ReadAsStringAsync();
 using var doc = JsonDocument.Parse(payload);
 if (doc.RootElement.TryGetProperty("access_token", out var at)) _accessToken = at.GetString();
 if (doc.RootElement.TryGetProperty("instance_url", out var iu)) _instanceUrl = iu.GetString();

 if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_instanceUrl))
 throw new Exception("Invalid authentication response from Salesforce");

 _logger.LogInformation("Authenticated with Salesforce; instance: {Instance}", _instanceUrl);
 }

 public async Task<HttpResponseMessage> PostAsync(string relativeUrl, string contentJson)
 {
 await EnsureAuthenticatedAsync();
 var client = _httpClientFactory.CreateClient("salesforce");
 client.BaseAddress = new Uri(_instanceUrl);
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
 var content = new StringContent(contentJson ?? string.Empty, Encoding.UTF8, "application/json");
 return await client.PostAsync(relativeUrl, content);
 }

 public async Task<HttpResponseMessage> GetAsync(string relativeUrl)
 {
 await EnsureAuthenticatedAsync();
 var client = _httpClientFactory.CreateClient("salesforce");
 client.BaseAddress = new Uri(_instanceUrl);
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
 return await client.GetAsync(relativeUrl);
 }

 public async Task<HttpResponseMessage> QueryAsync(string soql)
 {
 await EnsureAuthenticatedAsync();
 var client = _httpClientFactory.CreateClient("salesforce");
 client.BaseAddress = new Uri(_instanceUrl);
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
 var url = $"/services/data/v54.0/query?q={Uri.EscapeDataString(soql)}";
 return await client.GetAsync(url);
 }
 }
}
