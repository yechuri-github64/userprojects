using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using ManageordersLambda.Models;

namespace ManageordersLambda.Services
{
    public class Service
    {
        private readonly IConfiguration _config;
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public Service(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        private string GetConfigValue(string key) => _config.GetSection("Salesforce")[key] ?? string.Empty;

        private bool GetUseSandbox()
        {
            var v = GetConfigValue("UseSandbox");
            return bool.TryParse(v, out var b) && b;
        }

        private string GetTokenUrl()
        {
            var tokenUrl = GetConfigValue("TokenUrl");
            if (!string.IsNullOrEmpty(tokenUrl)) return tokenUrl;
            return GetUseSandbox() ? "https://test.salesforce.com/services/oauth2/token" : "https://login.salesforce.com/services/oauth2/token";
        }

        private string GetInstanceUrlFromConfig() => GetConfigValue("InstanceUrl");

        private HttpClient CreateHttpClient(TimeSpan? timeout = null)
        {
            var client = new HttpClient();
            client.Timeout = timeout ?? TimeSpan.FromSeconds(int.TryParse(GetConfigValue("TimeoutSeconds"), out var t) ? t : 120);
            return client;
        }

        private async Task<(string? accessToken, string? instanceUrl, string? error)> RetrieveAccessTokenAsync(ILambdaContext? context = null)
        {
            try
            {
                var tokenUrl = GetTokenUrl();
                using var client = CreateHttpClient();

                var clientId = GetConfigValue("ClientId");
                var clientSecret = GetConfigValue("ClientSecret");
                var username = GetConfigValue("Username");
                var password = GetConfigValue("Password");
                var securityToken = GetConfigValue("SecurityToken");

                var passwordWithToken = password + securityToken;

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("grant_type","password"),
                    new KeyValuePair<string,string>("client_id", clientId),
                    new KeyValuePair<string,string>("client_secret", clientSecret),
                    new KeyValuePair<string,string>("username", username),
                    new KeyValuePair<string,string>("password", passwordWithToken)
                });

                var response = await client.PostAsync(tokenUrl, content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    context?.Logger.LogLine($"Failed to retrieve token. Status: {response.StatusCode}. Body: {body}");
                    return (null, null, $"Token request failed: {body}");
                }

                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                if (root.TryGetProperty("access_token", out var at))
                {
                    var accessToken = at.GetString();
                    var instanceUrl = root.TryGetProperty("instance_url", out var iu) ? iu.GetString() : GetInstanceUrlFromConfig();
                    return (accessToken, instanceUrl, null);
                }

                return (null, null, "access_token not found in token response");
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine($"Exception in RetrieveAccessTokenAsync: {ex}");
                return (null, null, ex.Message);
            }
        }

        public async Task<Response> RetrieveOrderAsync(string orderId, ILambdaContext? context = null)
        {
            try
            {
                var (token, instanceUrl, tokenError) = await RetrieveAccessTokenAsync(context);
                if (!string.IsNullOrEmpty(tokenError) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(instanceUrl))
                {
                    context?.Logger.LogLine($"Token error: {tokenError}");
                    return new Response { Success = false, Error = new ErrorModel { Code = "AuthError", Message = tokenError ?? "Failed to obtain token" } };
                }

                using var client = CreateHttpClient();
                client.BaseAddress = new Uri(instanceUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Salesforce REST API for retrieving a record
                var apiVersion = GetConfigValue("ApiVersion");
                if (string.IsNullOrEmpty(apiVersion)) apiVersion = "59.0";

                var requestUri = $"/services/data/v{apiVersion}/sobjects/Order/{orderId}";
                context?.Logger.LogLine($"GET {client.BaseAddress}{requestUri}");

                var response = await client.GetAsync(requestUri);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    context?.Logger.LogLine($"RetrieveOrder failed. Status: {response.StatusCode}. Body: {body}");
                    return new Response { Success = false, Error = new ErrorModel { Code = response.StatusCode.ToString(), Message = body } };
                }

                var json = JsonDocument.Parse(body).RootElement;
                return new Response { Success = true, Data = json };
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine($"Exception in RetrieveOrderAsync: {ex}");
                return new Response { Success = false, Error = new ErrorModel { Code = "Exception", Message = ex.Message } };
            }
        }

        public async Task<Response> UpdateOrderAsync(string orderId, JsonElement data, ILambdaContext? context = null)
        {
            try
            {
                var (token, instanceUrl, tokenError) = await RetrieveAccessTokenAsync(context);
                if (!string.IsNullOrEmpty(tokenError) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(instanceUrl))
                {
                    context?.Logger.LogLine($"Token error: {tokenError}");
                    return new Response { Success = false, Error = new ErrorModel { Code = "AuthError", Message = tokenError ?? "Failed to obtain token" } };
                }

                using var client = CreateHttpClient();
                client.BaseAddress = new Uri(instanceUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var apiVersion = GetConfigValue("ApiVersion");
                if (string.IsNullOrEmpty(apiVersion)) apiVersion = "59.0";

                var requestUri = $"/services/data/v{apiVersion}/sobjects/Order/{orderId}";

                // Salesforce expects PATCH with JSON body
                var jsonPayload = data.GetRawText();
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) { Content = content };
                context?.Logger.LogLine($"PATCH {client.BaseAddress}{requestUri} Body: {jsonPayload}");

                var response = await client.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    // On success, Salesforce returns 204 No Content for PATCH
                    return new Response { Success = true, Data = JsonDocument.Parse("{} ").RootElement };
                }

                context?.Logger.LogLine($"UpdateOrder failed. Status: {response.StatusCode}. Body: {body}");
                return new Response { Success = false, Error = new ErrorModel { Code = response.StatusCode.ToString(), Message = body } };
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine($"Exception in UpdateOrderAsync: {ex}");
                return new Response { Success = false, Error = new ErrorModel { Code = "Exception", Message = ex.Message } };
            }
        }
    }
}
