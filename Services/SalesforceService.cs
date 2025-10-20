using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using accounts_sf_sa.Data;
using accounts_sf_sa.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace accounts_sf_sa.Services
{
    public class SalesforceService : ISalesforceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SalesforceService> _logger;
        private readonly SalesforceOptions _options;
        private readonly ApplicationDbContext _db;

        private class TokenState
        {
            public string AccessToken { get; set; } = string.Empty;
            public string InstanceUrl { get; set; } = string.Empty;
            public DateTimeOffset ExpiresAtUtc { get; set; }
        }

        private TokenState? _token;
        private readonly SemaphoreSlim _tokenLock = new(1, 1);

        public SalesforceService(
            IHttpClientFactory httpClientFactory,
            ILogger<SalesforceService> logger,
            IOptions<SalesforceOptions> options,
            ApplicationDbContext db
        )
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = options.Value;
            _db = db;
        }

        public async Task<List<CreateResult>> CreateMultipleAccountsAsync(
            List<Dictionary<string, object>> accounts
        )
        {
            var results = new List<CreateResult>();
            foreach (var acct in accounts)
            {
                var res = new CreateResult { Success = false };
                try
                {
                    var (client, baseUrl) = await CreateAuthedClientAsync();
                    var url = $"{baseUrl}/sobjects/Account";
                    var payload = JsonSerializer.Serialize(acct);
                    var httpRes = await client.PostAsync(
                        url,
                        new StringContent(payload, Encoding.UTF8, "application/json")
                    );
                    var content = await httpRes.Content.ReadAsStringAsync();

                    if (httpRes.IsSuccessStatusCode)
                    {
                        using var doc = JsonDocument.Parse(content);
                        res.Id = doc.RootElement.GetProperty("id").GetString() ?? string.Empty;
                        res.Success = doc.RootElement.GetProperty("success").GetBoolean();
                        await LogAsync("CREATE", res.Success, res.Id, "");
                    }
                    else
                    {
                        res.Errors.AddRange(ParseSalesforceErrors(content));
                        await LogAsync("CREATE", false, "", string.Join("; ", res.Errors));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating account");
                    res.Errors.Add(ex.Message);
                    await LogAsync("CREATE", false, "", ex.Message);
                }
                results.Add(res);
            }
            return results;
        }

        public async Task<Dictionary<string, object>?> GetAccountAsync(string id)
        {
            try
            {
                var (client, baseUrl) = await CreateAuthedClientAsync();
                var url = $"{baseUrl}/sobjects/Account/{id}";
                _logger.LogInformation("Salesforce URL: {url}", url);
                var httpRes = await client.GetAsync(url);
                var content = await httpRes.Content.ReadAsStringAsync();
                if (httpRes.IsSuccessStatusCode)
                {
                    await LogAsync("GET", true, id, "");
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                    return dict;
                }
                else
                {
                    await LogAsync(
                        "GET",
                        false,
                        id,
                        string.Join("; ", ParseSalesforceErrors(content))
                    );
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving account {Id}", id);
                await LogAsync("GET", false, id, ex.Message);
                return null;
            }
        }

        public async Task<bool> UpdateAccountAsync(string id, Dictionary<string, object> fields)
        {
            try
            {
                var (client, baseUrl) = await CreateAuthedClientAsync();
                var url = $"{baseUrl}/sobjects/Account/{id}";
                var payload = JsonSerializer.Serialize(fields);
                var req = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                {
                    Content = new StringContent(payload, Encoding.UTF8, "application/json"),
                };
                var httpRes = await client.SendAsync(req);
                var content = await httpRes.Content.ReadAsStringAsync();
                var ok = httpRes.IsSuccessStatusCode;
                await LogAsync(
                    "UPDATE",
                    ok,
                    id,
                    ok ? "" : string.Join("; ", ParseSalesforceErrors(content))
                );
                return ok;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account {Id}", id);
                await LogAsync("UPDATE", false, id, ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteAccountAsync(string id)
        {
            try
            {
                var (client, baseUrl) = await CreateAuthedClientAsync();
                var url = $"{baseUrl}/sobjects/Account/{id}";
                var httpRes = await client.DeleteAsync(url);
                var content = await httpRes.Content.ReadAsStringAsync();
                var ok = httpRes.IsSuccessStatusCode;
                await LogAsync(
                    "DELETE",
                    ok,
                    id,
                    ok ? "" : string.Join("; ", ParseSalesforceErrors(content))
                );
                return ok;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account {Id}", id);
                await LogAsync("DELETE", false, id, ex.Message);
                return false;
            }
        }

        private async Task<(HttpClient client, string baseUrl)> CreateAuthedClientAsync()
        { 
          _logger.LogInformation("In Salesforce token acquisition started." );
            var token = await GetTokenAsync();
            _logger.LogInformation("Salesforce token: {token}", token);
            var client = _httpClientFactory.CreateClient("Salesforce");
            client.Timeout = TimeSpan.FromSeconds(Math.Max(5, _options.TimeoutSeconds));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token.AccessToken
            );
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
            var baseUrl = $"{token.InstanceUrl}/services/data/v{_options.ApiVersion}";
            _logger.LogInformation("Salesforce base URL for API calls: {BaseUrl}", baseUrl);
            return (client, baseUrl);
        }

        private async Task<TokenState> GetTokenAsync()
        {
            if (_token != null && _token.ExpiresAtUtc > DateTimeOffset.UtcNow.AddMinutes(1))
            {
                return _token;
            }

            await _tokenLock.WaitAsync();
            try
            {
                if (_token != null && _token.ExpiresAtUtc > DateTimeOffset.UtcNow.AddMinutes(1))
                {
                    return _token;
                }

                var tokenUrl = !string.IsNullOrWhiteSpace(_options.TokenUrl)
                    ? _options.TokenUrl
                    : (
                        _options.UseSandbox
                            ? "https://test.salesforce.com/services/oauth2/token"
                            : "https://login.salesforce.com/services/oauth2/token"
                    );

                var client = _httpClientFactory.CreateClient("Salesforce");
                var content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        ["grant_type"] = "password",
                        ["client_id"] = _options.ClientId,
                        ["client_secret"] = _options.ClientSecret,
                        ["username"] = _options.Username,
                        ["password"] = _options.Password + _options.SecurityToken,
                    }
                );
                 _logger.LogInformation("Salesforce token URL for API calls: {tokenUrl}", tokenUrl);
                  _logger.LogInformation("Salesforce Content for the request: {content}", content);
                var res = await client.PostAsync(tokenUrl, content);
                var payload = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                {
                    var errors = string.Join("; ", ParseSalesforceErrors(payload));
                    throw new InvalidOperationException($"Salesforce auth failed: {errors}");
                }

                using var doc = JsonDocument.Parse(payload);
                var accessToken =
                    doc.RootElement.GetProperty("access_token").GetString() ?? string.Empty;
                var instanceUrl = !string.IsNullOrWhiteSpace(_options.InstanceUrl)
                    ? _options.InstanceUrl
                    : (
                        doc.RootElement.TryGetProperty("instance_url", out var inst)
                            ? inst.GetString() ?? string.Empty
                            : string.Empty
                    );
                var expiresIn = doc.RootElement.TryGetProperty("issued_at", out var issuedAt)
                    ? 600
                    : (
                        doc.RootElement.TryGetProperty("expires_in", out var exp)
                            ? exp.GetInt32()
                            : 3600
                    );

                _token = new TokenState
                {
                    AccessToken = accessToken,
                    InstanceUrl = instanceUrl,
                    ExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(expiresIn - 60),
                };

                await LogAsync("AUTH", true, "", "Token acquired");
                _logger.LogInformation("Salesforce token for API calls: {_token}", _token);
                return _token;
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        private static List<string> ParseSalesforceErrors(string payload)
        {
            var list = new List<string>();
            if (string.IsNullOrWhiteSpace(payload))
                return list;
            try
            {
                using var doc = JsonDocument.Parse(payload);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in doc.RootElement.EnumerateArray())
                    {
                        var msg = item.TryGetProperty("message", out var m) ? m.GetString() : null;
                        var code = item.TryGetProperty("errorCode", out var c)
                            ? c.GetString()
                            : null;
                        if (!string.IsNullOrWhiteSpace(msg) || !string.IsNullOrWhiteSpace(code))
                        {
                            list.Add($"{code}: {msg}");
                        }
                    }
                }
                else if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    var msg = doc.RootElement.TryGetProperty("message", out var m)
                        ? m.GetString()
                        : null;
                    if (!string.IsNullOrWhiteSpace(msg))
                        list.Add(msg);
                }
            }
            catch
            {
                list.Add(payload);
            }
            return list;
        }

        private async Task LogAsync(
            string operation,
            bool success,
            string referenceId,
            string details
        )
        {
            try
            {
                _db.RequestLogs.Add(
                    new RequestLog
                    {
                        Operation = operation,
                        Success = success,
                        ReferenceId = referenceId ?? string.Empty,
                        Details = details ?? string.Empty,
                        TimestampUtc = DateTimeOffset.UtcNow,
                    }
                );
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to persist RequestLog");
            }
        }
    }
}
