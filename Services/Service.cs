using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.Lambda.Core;
using TestSfLambdaLambda.Models;

namespace TestSfLambdaLambda.Services
{
    public class Service
    {
        private readonly string _instanceUrl;
        private readonly string _apiVersion;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _username;
        private readonly string _password;
        private readonly string _securityToken;
        private readonly HttpClient _httpClient;

        public Service(IConfiguration configuration)
        {
            _instanceUrl = configuration["Salesforce:InstanceUrl"] ?? string.Empty;
            _apiVersion = configuration["Salesforce:ApiVersion"] ?? "57.0";
            _clientId = configuration["Salesforce:ClientId"] ?? string.Empty;
            _clientSecret = configuration["Salesforce:ClientSecret"] ?? string.Empty;
            _username = configuration["Salesforce:Username"] ?? string.Empty;
            _password = configuration["Salesforce:Password"] ?? string.Empty;
            _securityToken = configuration["Salesforce:SecurityToken"] ?? string.Empty;
            _httpClient = new HttpClient();
        }

        private async Task<string?> GetAccessTokenAsync(ILambdaContext? context = null)
        {
            try
            {
                var tokenEndpoint = $"{_instanceUrl}/services/oauth2/token";
                var body = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string,string>("grant_type","password"),
                    new KeyValuePair<string,string>("client_id", _clientId),
                    new KeyValuePair<string,string>("client_secret", _clientSecret),
                    new KeyValuePair<string,string>("username", _username),
                    new KeyValuePair<string,string>("password", _password + _securityToken)
                };

                var req = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint) { Content = new FormUrlEncodedContent(body) };
                var resp = await _httpClient.SendAsync(req);
                var content = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    context?.Logger.LogLine($"GetAccessToken failed: {content}");
                    return null;
                }

                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("access_token", out var t))
                {
                    return t.GetString();
                }

                return null;
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine($"GetAccessToken exception: {ex}");
                return null;
            }
        }

        public async Task<Response> CreateAccountsAsync(Request request, ILambdaContext? context = null)
        {
            try
            {
                if (request?.Accounts == null || request.Accounts.Count == 0)
                {
                    return new Response(false, null, new ErrorDetail("Validation", "No accounts provided for creation"));
                }

                var token = await GetAccessTokenAsync(context);
                if (string.IsNullOrEmpty(token))
                {
                    return new Response(false, null, new ErrorDetail("Auth", "Unable to obtain access token"));
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var created = new List<object>();
                foreach (var acc in request.Accounts)
                {
                    var payload = new Dictionary<string, object?>
                    {
                        { "Name", acc.Name },
                        { "Phone", acc.Phone },
                        { "Industry", acc.Industry }
                    };

                    var uri = $"{_instanceUrl}/services/data/v{_apiVersion}/sobjects/Account";
                    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                    var resp = await _httpClient.PostAsync(uri, content);
                    var respContent = await resp.Content.ReadAsStringAsync();
                    if (!resp.IsSuccessStatusCode)
                    {
                        context?.Logger.LogLine($"CreateAccount failed: {respContent}");
                        created.Add(new { Input = acc, Error = respContent });
                    }
                    else
                    {
                        using var doc = JsonDocument.Parse(respContent);
                        var obj = new Dictionary<string, object?>();
                        if (doc.RootElement.TryGetProperty("id", out var idProp)) obj["id"] = idProp.GetString();
                        if (doc.RootElement.TryGetProperty("success", out var sProp)) obj["success"] = sProp.GetBoolean();
                        created.Add(new { Input = acc, Result = obj });
                    }
                }

                return new Response(true, created, null);
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine($"CreateAccountsAsync exception: {ex}");
                return new Response(false, null, new ErrorDetail("Exception", ex.Message));
            }
        }

        public async Task<Response> GetAccountsAsync(Request request, ILambdaContext? context = null)
{
    try
    {
        var token = await GetAccessTokenAsync(context);
        if (string.IsNullOrEmpty(token))
        {
            return new Response(false, null, new ErrorDetail("Auth", "Unable to obtain access token"));
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (!string.IsNullOrEmpty(request?.AccountId))
        {
            var uri = $"{_instanceUrl}/services/data/v{_apiVersion}/sobjects/Account/{request.AccountId}";
            var resp = await _httpClient.GetAsync(uri);
            var content = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                context?.Logger.LogLine($"GetAccount failed: {content}");
                return new Response(false, null, new ErrorDetail("GetFailed", content));
            }

            using var doc = JsonDocument.Parse(content);
            var clone = JsonDocument.Parse(doc.RootElement.GetRawText()).RootElement.Clone();
            return new Response(true, clone, null);
        }
        else
        {
            var soql = "SELECT Id, Name, Phone, Industry FROM Account LIMIT 200";
            var uri = $"{_instanceUrl}/services/data/v{_apiVersion}/query?q={Uri.EscapeDataString(soql)}";
            var resp = await _httpClient.GetAsync(uri);
            var content = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                context?.Logger.LogLine($"QueryAccounts failed: {content}");
                return new Response(false, null, new ErrorDetail("QueryFailed", content));
            }

            using var doc = JsonDocument.Parse(content);
            var clone = JsonDocument.Parse(doc.RootElement.GetRawText()).RootElement.Clone();
            return new Response(true, clone, null);
        }
    }
    catch (Exception ex)
    {
        context?.Logger.LogLine($"GetAccountsAsync exception: {ex}");
        return new Response(false, null, new ErrorDetail("Exception", ex.Message));
    }
}

        public async Task<Response> UpdateAccountAsync(Request request, ILambdaContext? context = null)
        {
            try
            {
                if (request?.Account == null || string.IsNullOrEmpty(request.AccountId))
                {
                    return new Response(false, null, new ErrorDetail("Validation", "Account and AccountId required for update"));
                }

                var token = await GetAccessTokenAsync(context);
                if (string.IsNullOrEmpty(token))
                {
                    return new Response(false, null, new ErrorDetail("Auth", "Unable to obtain access token"));
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var payload = new Dictionary<string, object?>
                {
                    { "Name", request.Account.Name },
                    { "Phone", request.Account.Phone },
                    { "Industry", request.Account.Industry }
                };

                var uri = $"{_instanceUrl}/services/data/v{_apiVersion}/sobjects/Account/{request.AccountId}";
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var requestMsg = new HttpRequestMessage(new HttpMethod("PATCH"), uri) { Content = content };
                var resp = await _httpClient.SendAsync(requestMsg);
                var respContent = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    context?.Logger.LogLine($"UpdateAccount failed: {respContent}");
                    return new Response(false, null, new ErrorDetail("UpdateFailed", respContent));
                }

                return new Response(true, new { AccountId = request.AccountId }, null);
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine($"UpdateAccountAsync exception: {ex}");
                return new Response(false, null, new ErrorDetail("Exception", ex.Message));
            }
        }

        public async Task<Response> DeleteAccountAsync(Request request, ILambdaContext? context = null)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.AccountId))
                {
                    return new Response(false, null, new ErrorDetail("Validation", "AccountId required for delete"));
                }

                var token = await GetAccessTokenAsync(context);
                if (string.IsNullOrEmpty(token))
                {
                    return new Response(false, null, new ErrorDetail("Auth", "Unable to obtain access token"));
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var uri = $"{_instanceUrl}/services/data/v{_apiVersion}/sobjects/Account/{request.AccountId}";
                var resp = await _httpClient.DeleteAsync(uri);
                var respContent = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    context?.Logger.LogLine($"DeleteAccount failed: {respContent}");
                    return new Response(false, null, new ErrorDetail("DeleteFailed", respContent));
                }

                return new Response(true, new { AccountId = request.AccountId }, null);
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine($"DeleteAccountAsync exception: {ex}");
                return new Response(false, null, new ErrorDetail("Exception", ex.Message));
            }
        }
    }
}
