using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using test-acc-sf-app.Models;

namespace test-acc-sf-app.Services
{
 public class SalesforceClient : ISalesforceClient
 {
 private readonly HttpClient _httpClient;
 private readonly SalesforceSettings _settings;
 private readonly ILogger<SalesforceClient> _logger;
 private string _accessToken;
 private DateTime _accessTokenExpiresAt;
 private readonly object _lock = new object();

 public SalesforceClient(HttpClient httpClient, IOptions<SalesforceSettings> options, ILogger<SalesforceClient> logger)
 {
 _httpClient = httpClient;
 _settings = options.Value;
 _logger = logger;

 if (!string.IsNullOrEmpty(_settings.InstanceUrl))
 {
 _httpClient.BaseAddress = new Uri(_settings.InstanceUrl);
 }
 }

 private async Task EnsureAuthenticatedAsync()
 {
 if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _accessTokenExpiresAt)
 return;

 lock (_lock)
 {
 if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _accessTokenExpiresAt)
 return;

 // continue to request token outside lock to avoid deadlocks
 }

 var tokenEndpoint = "/services/oauth2/token";

 var content = new FormUrlEncodedContent(new[]
 {
 new KeyValuePair<string, string>("grant_type", "password"),
 new KeyValuePair<string, string>("client_id", _settings.ClientId ?? string.Empty),
 new KeyValuePair<string, string>("client_secret", _settings.ClientSecret ?? string.Empty),
 new KeyValuePair<string, string>("username", _settings.Username ?? string.Empty),
 new KeyValuePair<string, string>("password", _settings.Password ?? string.Empty)
 });

 var resp = await _httpClient.PostAsync(tokenEndpoint, content);
 resp.EnsureSuccessStatusCode();

 var json = await resp.Content.ReadAsStringAsync();
 using var doc = JsonDocument.Parse(json);
 var root = doc.RootElement;
 _accessToken = root.GetProperty("access_token").GetString();
 var issuedAt = DateTime.UtcNow;
 var expiresIn = root.TryGetProperty("expires_in", out var ei) ? ei.GetInt32() : 3600;
 _accessTokenExpiresAt = issuedAt.AddSeconds(expiresIn - 60);

 _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
 }

 public async Task<string> GetAsync(string path)
 {
 await EnsureAuthenticatedAsync();
 var resp = await _httpClient.GetAsync(NormalizePath(path));
 if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
 resp.EnsureSuccessStatusCode();
 return await resp.Content.ReadAsStringAsync();
 }

 public async Task<object> CreateAsync(string path, object payload)
 {
 await EnsureAuthenticatedAsync();
 var json = JsonSerializer.Serialize(payload);
 var resp = await _httpClient.PostAsync(NormalizePath(path), new StringContent(json, Encoding.UTF8, "application/json"));
 resp.EnsureSuccessStatusCode();
 var body = await resp.Content.ReadAsStringAsync();
 using var doc = JsonDocument.Parse(body);
 var root = doc.RootElement;
 if (root.TryGetProperty("id", out var idProp))
 {
 return new { id = idProp.GetString() };
 }

 return JsonSerializer.Deserialize<object>(body);
 }

 public async Task<bool> PatchAsync(string path, object payload)
 {
 await EnsureAuthenticatedAsync();
 var json = JsonSerializer.Serialize(payload);
 var request = new HttpRequestMessage(new HttpMethod("PATCH"), NormalizePath(path))
 {
 Content = new StringContent(json, Encoding.UTF8, "application/json")
 };

 var resp = await _httpClient.SendAsync(request);
 if (resp.StatusCode == System.Net.HttpStatusCode.NoContent) return true;
 if (resp.IsSuccessStatusCode) return true;
 return false;
 }

 public async Task<bool> DeleteAsync(string path)
 {
 await EnsureAuthenticatedAsync();
 var resp = await _httpClient.DeleteAsync(NormalizePath(path));
 if (resp.StatusCode == System.Net.HttpStatusCode.NoContent) return true;
 if (resp.IsSuccessStatusCode) return true;
 return false;
 }

 private string NormalizePath(string path)
 {
 if (string.IsNullOrWhiteSpace(path)) return string.Empty;
 if (path.StartsWith("/")) return path;
 return "/" + path;
 }
 }
}
