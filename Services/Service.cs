using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;

namespace GetaccountsLambda.Services
{
    public class Service
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly string _apiVersion;

        public Service(IConfiguration config)
        {
            _config = config;
            var timeoutSecondsStr = _config["Salesforce:TimeoutSeconds"] ?? "120";
            if (!int.TryParse(timeoutSecondsStr, out var timeoutSeconds)) timeoutSeconds = 120;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutSeconds) };
            _apiVersion = _config["Salesforce:ApiVersion"] ?? "59.0";
        }

        public async Task<Dictionary<string, object?>> GetAccountByIdAsync(string accountId, ILambdaContext context)
        {
            try
            {
                var tokenResponse = await AuthenticateAsync(context);
                if (tokenResponse == null || !tokenResponse.ContainsKey("access_token"))
                    throw new Exception("Failed to retrieve access token from Salesforce");

                var accessToken = tokenResponse["access_token"]?.ToString();
                var instanceUrl = tokenResponse.ContainsKey("instance_url") ? tokenResponse["instance_url"]?.ToString() : _config["Salesforce:InstanceUrl"];
                if (string.IsNullOrWhiteSpace(instanceUrl))
                    throw new Exception("Salesforce instance URL is not available");

                var requestUrl = $"{instanceUrl}/services/data/v{_apiVersion}/sobjects/Account/{accountId}";
                using var rq = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                rq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var resp = await _httpClient.SendAsync(rq);
                var content = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    context.Logger.LogLine($"Salesforce returned status {(int)resp.StatusCode}: {content}");
                    throw new Exception($"Salesforce returned {(int)resp.StatusCode}: {content}");
                }

                var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(content);
                var result = new Dictionary<string, object?>();
                if (dict != null)
                {
                    foreach (var kv in dict)
                    {
                        var val = JsonElementToObject(kv.Value);
                        // skip null or empty strings
                        if (val == null) continue;
                        if (val is string s && string.IsNullOrWhiteSpace(s)) continue;
                        result[kv.Key] = val;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error in GetAccountByIdAsync: {ex}");
                throw;
            }
        }

        private async Task<Dictionary<string, object?>?> AuthenticateAsync(ILambdaContext context)
        {
            try
            {
                var cfg = _config.GetSection("Salesforce");
                var tokenUrl = cfg["TokenUrl"];
                var useSandbox = bool.TryParse(cfg["UseSandbox"], out var us) ? us : false;
                if (string.IsNullOrWhiteSpace(tokenUrl))
                {
                    tokenUrl = useSandbox ? "https://test.salesforce.com/services/oauth2/token" : "https://login.salesforce.com/services/oauth2/token";
                }

                var clientId = cfg["ClientId"] ?? string.Empty;
                var clientSecret = cfg["ClientSecret"] ?? string.Empty;
                var username = cfg["Username"] ?? string.Empty;
                var password = cfg["Password"] ?? string.Empty;
                var securityToken = cfg["SecurityToken"] ?? string.Empty;

                var form = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type","password"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password + securityToken)
                };

                using var req = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
                {
                    Content = new FormUrlEncodedContent(form)
                };
                var resp = await _httpClient.SendAsync(req);
                var content = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    context.Logger.LogLine($"Token endpoint returned {(int)resp.StatusCode}: {content}");
                    throw new Exception($"Token endpoint returned {(int)resp.StatusCode}: {content}");
                }

                var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(content);
                var result = new Dictionary<string, object?>();
                if (dict != null)
                {
                    foreach (var kv in dict)
                        result[kv.Key] = JsonElementToObject(kv.Value);
                }
                return result;
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Authentication error: {ex}");
                throw;
            }
        }

        private object? JsonElementToObject(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String: return element.GetString();
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var l)) return l;
                    if (element.TryGetDouble(out var d)) return d;
                    return element.GetDecimal();
                case JsonValueKind.True: return true;
                case JsonValueKind.False: return false;
                case JsonValueKind.Object:
                    var dict = new Dictionary<string, object?>();
                    foreach (var prop in element.EnumerateObject())
                    {
                        var val = JsonElementToObject(prop.Value);
                        if (val == null) continue;
                        if (val is string s && string.IsNullOrWhiteSpace(s)) continue;
                        dict[prop.Name] = val;
                    }
                    return dict.Count > 0 ? dict : null;
                case JsonValueKind.Array:
                    var list = new List<object?>();
                    foreach (var item in element.EnumerateArray())
                    {
                        var v = JsonElementToObject(item);
                        list.Add(v);
                    }
                    return list.Count > 0 ? list : null;
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                default:
                    return null;
            }
        }
    }
}
