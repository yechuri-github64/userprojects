using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CreateaccountsdemoLambda.Models;
using Npgsql;

namespace CreateaccountsdemoLambda.Services
{
    public class Service
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;
        private readonly string _apiVersion;
        private readonly bool _useSandbox;
        private readonly string _tokenUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _username;
        private readonly string _password;
        private readonly string _securityToken;
        private readonly string _instanceUrlSetting;

        public Service(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            var sf = _config.GetSection("Salesforce");
            _apiVersion = sf["ApiVersion"] ?? "59.0";
            _useSandbox = string.Equals(sf["UseSandbox"], "true", StringComparison.OrdinalIgnoreCase);
            _tokenUrl = sf["TokenUrl"] ?? ( _useSandbox ? "https://test.salesforce.com/services/oauth2/token" : "https://login.salesforce.com/services/oauth2/token");
            _clientId = sf["ClientId"] ?? string.Empty;
            _clientSecret = sf["ClientSecret"] ?? string.Empty;
            _username = sf["Username"] ?? string.Empty;
            _password = sf["Password"] ?? string.Empty;
            _securityToken = sf["SecurityToken"] ?? string.Empty;
            _instanceUrlSetting = sf["InstanceUrl"] ?? string.Empty;

            _http = new HttpClient();
            _http.Timeout = TimeSpan.FromSeconds(int.TryParse(sf["TimeoutSeconds"], out var t) ? t : 120);

            // Register Npgsql enum mapping as required
            try { EnumMapping.RegisterEnums(); } catch { }
        }

        public async Task<Response> ProcessAsync(Request request)
        {
            try
            {
                var token = await GetAccessTokenAsync();
                if (!token.Success)
                {
                    return new Response { Success = false, Error = token.Error };
                }

                var accessToken = token.AccessToken!;
                var instanceUrl = token.InstanceUrl ?? _instanceUrlSetting;
                if (string.IsNullOrWhiteSpace(instanceUrl))
                {
                    return new Response
                    {
                        Success = false,
                        Error = new ErrorModel { Code = "NoInstanceUrl", Message = "InstanceUrl is not configured and could not be obtained from token." , Timestamp = DateTime.UtcNow}
                    };
                }

                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                _http.BaseAddress = new Uri(instanceUrl);

                switch (request.Operation)
                {
                    case Models.OperationType.Get:
                        return await HandleGet(request);
                    case Models.OperationType.Create:
                        return await HandleCreateMultiple(request);
                    case Models.OperationType.Update:
                        return await HandleUpdate(request);
                    case Models.OperationType.Delete:
                        return await HandleDelete(request);
                    default:
                        return new Response { Success = false, Error = new ErrorModel { Code = "InvalidOperation", Message = "Unsupported operation.", Timestamp = DateTime.UtcNow } };
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Success = false,
                    Error = new ErrorModel { Code = "ProcessingError", Message = ex.Message, Details = ex.ToString(), Timestamp = DateTime.UtcNow }
                };
            }
        }

        private async Task<(bool Success, string? AccessToken, string? InstanceUrl, ErrorModel? Error)> GetAccessTokenAsync()
        {
            try
            {
                // Password grant: password + security token concatenated if security token provided
                var pwd = _password ?? string.Empty;
                if (!string.IsNullOrEmpty(_securityToken)) pwd = pwd + _securityToken;

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("grant_type","password"),
                    new KeyValuePair<string,string>("client_id",_clientId),
                    new KeyValuePair<string,string>("client_secret",_clientSecret),
                    new KeyValuePair<string,string>("username",_username),
                    new KeyValuePair<string,string>("password",pwd)
                });

                using var resp = await _http.PostAsync(_tokenUrl, content);
                var s = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    return (false, null, null, new ErrorModel { Code = "AuthFailed", Message = "Failed to obtain access token.", Details = s, Timestamp = DateTime.UtcNow });
                }

                using var doc = JsonDocument.Parse(s);
                var root = doc.RootElement;
                var accessToken = root.GetProperty("access_token").GetString();
                var instanceUrl = root.TryGetProperty("instance_url", out var iu) ? iu.GetString() : null;
                return (true, accessToken, instanceUrl, null);
            }
            catch (Exception ex)
            {
                return (false, null, null, new ErrorModel { Code = "AuthException", Message = ex.Message, Details = ex.ToString(), Timestamp = DateTime.UtcNow });
            }
        }

        private async Task<Response> HandleGet(Request request)
        {
            try
            {
                var id = request.Record?.Id;
                if (string.IsNullOrWhiteSpace(id))
                {
                    return new Response { Success = false, Error = new ErrorModel { Code = "MissingId", Message = "Id is required for Get operation.", Timestamp = DateTime.UtcNow } };
                }

                var url = $"/services/data/v{_apiVersion}/sobjects/Account/{Uri.EscapeDataString(id)}";
                var res = await _http.GetAsync(url);
                var body = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                {
                    return new Response { Success = false, Error = new ErrorModel { Code = "GetFailed", Message = "Failed to get account.", Details = body, Timestamp = DateTime.UtcNow } };
                }
                var obj = JsonSerializer.Deserialize<object>(body);
                return new Response { Success = true, Data = obj };
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Error = new ErrorModel { Code = "GetException", Message = ex.Message, Details = ex.ToString(), Timestamp = DateTime.UtcNow } };
            }
        }

        private async Task<Response> HandleCreateMultiple(Request request)
        {
            try
            {
                var records = request.Records ?? new List<AccountModel>();
                if (records.Count == 0)
                {
                    return new Response { Success = false, Error = new ErrorModel { Code = "NoRecords", Message = "At least one record must be provided for Create.", Timestamp = DateTime.UtcNow } };
                }

                var results = new List<CreateResult>();

                // Create each record individually (simple approach). For production, consider Composite API for bulk operations.
                foreach (var rec in records)
                {
                    try
                    {
                        var payload = new Dictionary<string, object?>
                        {
                            ["Name"] = rec.Name,
                            ["Phone"] = null,
                            ["Description"] = null
                        };
                        if (!string.IsNullOrWhiteSpace(rec.Email)) payload["Email__c"] = rec.Email; // custom field mapping example
                        if (!string.IsNullOrWhiteSpace(rec.Address)) payload["BillingStreet"] = rec.Address;

                        var url = $"/services/data/v{_apiVersion}/sobjects/Account/";
                        var httpContent = JsonContent.Create(payload);
                        using var resp = await _http.PostAsync(url, httpContent);
                        var body = await resp.Content.ReadAsStringAsync();
                        if (!resp.IsSuccessStatusCode)
                        {
                            results.Add(new CreateResult { Success = false, Id = null, Errors = body });
                        }
                        else
                        {
                            using var doc = JsonDocument.Parse(body);
                            var id = doc.RootElement.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                            var success = doc.RootElement.TryGetProperty("success", out var s) && s.GetBoolean();
                            results.Add(new CreateResult { Id = id, Success = success, Errors = success ? null : body });
                        }
                    }
                    catch (Exception exRec)
                    {
                        results.Add(new CreateResult { Success = false, Errors = exRec.Message });
                    }
                }

                return new Response { Success = true, Data = results };
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Error = new ErrorModel { Code = "CreateException", Message = ex.Message, Details = ex.ToString(), Timestamp = DateTime.UtcNow } };
            }
        }

        private async Task<Response> HandleUpdate(Request request)
        {
            try
            {
                var rec = request.Record;
                if (rec == null || string.IsNullOrWhiteSpace(rec.Id))
                {
                    return new Response { Success = false, Error = new ErrorModel { Code = "MissingIdOrRecord", Message = "Record with Id is required for Update.", Timestamp = DateTime.UtcNow } };
                }

                var payload = new Dictionary<string, object?>();
                if (rec.Name != null) payload["Name"] = rec.Name;
                if (rec.Email != null) payload["Email__c"] = rec.Email;
                if (rec.Address != null) payload["BillingStreet"] = rec.Address;

                var url = $"/services/data/v{_apiVersion}/sobjects/Account/{Uri.EscapeDataString(rec.Id)}";
                var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                {
                    Content = JsonContent.Create(payload)
                };

                using var resp = await _http.SendAsync(requestMessage);
                var body = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    return new Response { Success = false, Error = new ErrorModel { Code = "UpdateFailed", Message = "Failed to update account.", Details = body, Timestamp = DateTime.UtcNow } };
                }

                return new Response { Success = true, Data = new { UpdatedId = rec.Id } };
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Error = new ErrorModel { Code = "UpdateException", Message = ex.Message, Details = ex.ToString(), Timestamp = DateTime.UtcNow } };
            }
        }

        private async Task<Response> HandleDelete(Request request)
        {
            try
            {
                var id = request.Record?.Id;
                if (string.IsNullOrWhiteSpace(id))
                {
                    return new Response { Success = false, Error = new ErrorModel { Code = "MissingId", Message = "Id is required for Delete operation.", Timestamp = DateTime.UtcNow } };
                }

                var url = $"/services/data/v{_apiVersion}/sobjects/Account/{Uri.EscapeDataString(id)}";
                using var resp = await _http.DeleteAsync(url);
                var body = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    return new Response { Success = false, Error = new ErrorModel { Code = "DeleteFailed", Message = "Failed to delete account.", Details = body, Timestamp = DateTime.UtcNow } };
                }

                return new Response { Success = true, Data = new { DeletedId = id } };
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Error = new ErrorModel { Code = "DeleteException", Message = ex.Message, Details = ex.ToString(), Timestamp = DateTime.UtcNow } };
            }
        }

        // Simple helper to parse token response container
        private class TokenResponse
        {
            public string? access_token { get; set; }
            public string? instance_url { get; set; }
            public string? id { get; set; }
            public string? token_type { get; set; }
            public string? issued_at { get; set; }
            public string? signature { get; set; }
        }
    }
}
