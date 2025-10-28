using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TestsflambdaLambda.Models;

namespace TestsflambdaLambda.Services
{
    public class Service
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly SalesforceSettings _settings;
        private string _accessToken = string.Empty;
        private string _instanceUrl = string.Empty;

        public Service(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _settings = new SalesforceSettings(configuration.GetSection("Salesforce"));
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        }

        private async Task AuthenticateAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && !string.IsNullOrEmpty(_instanceUrl))
            {
                return; // already authenticated for lambda lifecycle
            }

            var tokenUrl = _settings.TokenUrl;

            var payload = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", _settings.ClientId },
                { "client_secret", _settings.ClientSecret },
                { "username", _settings.Username },
                { "password", _settings.Password + _settings.SecurityToken }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
            {
                Content = new FormUrlEncodedContent(payload)
            };

            var resp = await _httpClient.SendAsync(request);
            var content = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                throw new ServiceException((int)resp.StatusCode, "Failed to obtain Salesforce token", content);
            }

            using var doc = JsonDocument.Parse(content);
            if (doc.RootElement.TryGetProperty("access_token", out var at))
            {
                _accessToken = at.GetString() ?? string.Empty;
            }
            if (doc.RootElement.TryGetProperty("instance_url", out var iu))
            {
                _instanceUrl = iu.GetString() ?? _settings.InstanceUrl;
            }

            if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_instanceUrl))
            {
                throw new ServiceException(500, "Invalid token response", content);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _httpClient.BaseAddress = new Uri(_instanceUrl);
        }

        public async Task<List<object>> CreateAccountsAsync(List<AccountDto> accounts)
        {
            await AuthenticateAsync();
            var results = new List<object>();

            foreach (var account in accounts)
            {
                var body = new Dictionary<string, object?>
                {
                    { "Name", account.Name },
                    { "Phone", account.Phone },
                    { "Website", account.Website }
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var resp = await _httpClient.PostAsync($"/services/data/v{_settings.ApiVersion}/sobjects/Account/", content);
                var respContent = await resp.Content.ReadAsStringAsync();

                if (resp.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(respContent);
                    var id = doc.RootElement.GetProperty("id").GetString();
                    results.Add(new { id, success = true });
                }
                else
                {
                    // collect error message
                    results.Add(new { id = (string?)null, success = false, error = respContent });
                }
            }

            return results;
        }

        public async Task<object> GetAccountAsync(string id)
        {
            await AuthenticateAsync();
            var resp = await _httpClient.GetAsync($"/services/data/v{_settings.ApiVersion}/sobjects/Account/{id}");
            var content = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                throw new ServiceException((int)resp.StatusCode, "Failed to retrieve account", content);
            }

            using var doc = JsonDocument.Parse(content);
            return JsonSerializer.Deserialize<object>(doc.RootElement.GetRawText())!;
        }

        public async Task UpdateAccountAsync(AccountDto account)
        {
            await AuthenticateAsync();
            var body = new Dictionary<string, object?>
            {
                { "Name", account.Name },
                { "Phone", account.Phone },
                { "Website", account.Website }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"/services/data/v{_settings.ApiVersion}/sobjects/Account/{account.Id}")
            {
                Content = content
            };

            var resp = await _httpClient.SendAsync(request);
            var respContent = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode && resp.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw new ServiceException((int)resp.StatusCode, "Failed to update account", respContent);
            }
        }

        public async Task DeleteAccountAsync(string id)
        {
            await AuthenticateAsync();
            var resp = await _httpClient.DeleteAsync($"/services/data/v{_settings.ApiVersion}/sobjects/Account/{id}");
            var respContent = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode && resp.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw new ServiceException((int)resp.StatusCode, "Failed to delete account", respContent);
            }
        }

        private class SalesforceSettings
        {
            public string Provider { get; }
            public string ApiVersion { get; }
            public bool UseSandbox { get; }
            public string ClientId { get; }
            public string ClientSecret { get; }
            public string Username { get; }
            public string Password { get; }
            public string SecurityToken { get; }
            public string TokenUrl { get; }
            public string AuthUrl { get; }
            public string InstanceUrl { get; }
            public int TimeoutSeconds { get; }
            public bool EnableLogging { get; }
            public int MaxRetries { get; }
            public int DelaySeconds { get; }

            public SalesforceSettings(IConfigurationSection section)
            {
                Provider = section["Provider"] ?? string.Empty;
                ApiVersion = section["ApiVersion"] ?? "59.0";
                UseSandbox = bool.TryParse(section["UseSandbox"], out var us) && us;
                ClientId = section["ClientId"] ?? string.Empty;
                ClientSecret = section["ClientSecret"] ?? string.Empty;
                Username = section["Username"] ?? string.Empty;
                Password = section["Password"] ?? string.Empty;
                SecurityToken = section["SecurityToken"] ?? string.Empty;
                TokenUrl = section["TokenUrl"] ?? (UseSandbox ? "https://test.salesforce.com/services/oauth2/token" : "https://login.salesforce.com/services/oauth2/token");
                AuthUrl = section["AuthUrl"] ?? (UseSandbox ? "https://test.salesforce.com" : "https://login.salesforce.com");
                InstanceUrl = section["InstanceUrl"] ?? string.Empty;
                TimeoutSeconds = int.TryParse(section["TimeoutSeconds"], out var t) ? t : 120;
                EnableLogging = bool.TryParse(section["EnableLogging"], out var el) && el;
                MaxRetries = int.TryParse(section["RetryPolicy:MaxRetries"], out var mr) ? mr : 3;
                DelaySeconds = int.TryParse(section["RetryPolicy:DelaySeconds"], out var ds) ? ds : 3;
            }
        }
    }
}
