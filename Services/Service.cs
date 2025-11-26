using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CreateaccountsdemoLambda.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Amazon.Lambda.Core;

namespace CreateaccountsdemoLambda.Services
{
    public class Service
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _apiVersion;

        static Service()
        {
            // Register Npgsql enum mapping
            try
            {
                NpgsqlConnection.GlobalTypeMapper.MapEnum<RailcardType>();
            }
            catch
            {
                // ignore mapping errors at startup
            }
        }

        public Service()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
            _httpClient = new HttpClient();
            _apiVersion = _configuration["Salesforce:ApiVersion"] ?? "59.0";
        }

        private async Task<(string? AccessToken, string? InstanceUrl, string? Error)> AuthenticateAsync()
        {
            try
            {
                var section = _configuration.GetSection("Salesforce");
                var tokenUrl = section["TokenUrl"] ?? string.Empty;
                var clientId = section["ClientId"] ?? string.Empty;
                var clientSecret = section["ClientSecret"] ?? string.Empty;
                var username = section["Username"] ?? string.Empty;
                var password = section["Password"] ?? string.Empty;
                var securityToken = section["SecurityToken"] ?? string.Empty;

                var form = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "username", username },
                    { "password", password + securityToken }
                };

                using var content = new FormUrlEncodedContent(form);
                using var res = await _httpClient.PostAsync(tokenUrl, content);
                var txt = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                {
                    return (null, null, txt);
                }

                using var doc = JsonDocument.Parse(txt);
                var root = doc.RootElement;

                var accessToken = root.GetProperty("access_token").GetString();
                var instanceUrl = root.GetProperty("instance_url").GetString();

                return (accessToken, instanceUrl, null);
            }
            catch (Exception ex)
            {
                return (null, null, ex.ToString());
            }
        }

        public async Task<Response> GetAccountAsync(Request request, ILambdaContext? context = null)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Id))
                {
                    return Response.ErrorResponse("Id is required for Get action");
                }

                var (token, instanceUrl, error) = await AuthenticateAsync();
                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(instanceUrl))
                {
                    return Response.ErrorResponse("Authentication failed", error);
                }

                using var client = new HttpClient { BaseAddress = new Uri(instanceUrl) };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var url = $"/services/data/v{_apiVersion}/sobjects/Account/{request.Id}";
                var res = await client.GetAsync(url);
                var txt = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                {
                    return Response.ErrorResponse("Failed to retrieve account", txt);
                }

                using var doc = JsonDocument.Parse(txt);
                var root = doc.RootElement;

                var output = new AccountOutput
                {
                    Id = root.GetProperty("Id").GetString(),
                    Name = root.TryGetProperty("Name", out var nm) ? nm.GetString() : null,
                    Email = root.TryGetProperty("PersonEmail", out var em) ? em.GetString() : null,
                    Address = root.TryGetProperty("BillingStreet", out var addr) ? addr.GetString() : null
                };

                return new Response { Success = true, Accounts = new List<AccountOutput> { output } };
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine(ex.ToString());
                return Response.FromException(ex);
            }
        }

        public async Task<Response> CreateAccountsAsync(Request request, ILambdaContext? context = null)
        {
            try
            {
                if (request.Accounts == null || request.Accounts.Count == 0)
                {
                    return Response.ErrorResponse("No accounts provided for creation");
                }

                var (token, instanceUrl, error) = await AuthenticateAsync();
                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(instanceUrl))
                {
                    return Response.ErrorResponse("Authentication failed", error);
                }

                using var client = new HttpClient { BaseAddress = new Uri(instanceUrl) };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Use composite sobjects endpoint to create multiple records in one request
                var records = new List<object>();
                foreach (var acc in request.Accounts)
                {
                    var rec = new Dictionary<string, object>
                    {
                        { "attributes", new Dictionary<string,string> { { "type", "Account" } } },
                        { "Name", acc.Name ?? string.Empty }
                    };

                    if (!string.IsNullOrEmpty(acc.Email)) rec["PersonEmail"] = acc.Email;
                    if (!string.IsNullOrEmpty(acc.Address)) rec["BillingStreet"] = acc.Address;

                    records.Add(rec);
                }

                var payload = new Dictionary<string, object>
                {
                    { "allOrNone", false },
                    { "records", records }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"/services/data/v{_apiVersion}/composite/sobjects";
                var res = await client.PostAsync(url, content);
                var txt = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                {
                    return Response.ErrorResponse("Failed to create accounts", txt);
                }

                var outputs = new List<AccountOutput>();
                using (var doc = JsonDocument.Parse(txt))
                {
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        int i = 0;
                        foreach (var item in root.EnumerateArray())
                        {
                            var success = item.GetProperty("success").GetBoolean();
                            if (success)
                            {
                                var id = item.GetProperty("id").GetString();
                                var input = request.Accounts.Count > i ? request.Accounts[i] : null;
                                outputs.Add(new AccountOutput { Id = id, Name = input?.Name, Email = input?.Email, Address = input?.Address });
                            }
                            i++;
                        }
                    }
                }

                return new Response { Success = true, Accounts = outputs };
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine(ex.ToString());
                return Response.FromException(ex);
            }
        }

        public async Task<Response> UpdateAccountAsync(Request request, ILambdaContext? context = null)
        {
            try
            {
                // Update only one account at a time
                if (request.Accounts == null || request.Accounts.Count == 0)
                    return Response.ErrorResponse("No account provided for update");

                var acc = request.Accounts[0];
                if (string.IsNullOrEmpty(acc.Id) && string.IsNullOrEmpty(request.Id))
                    return Response.ErrorResponse("Id is required to update account");

                var id = acc.Id ?? request.Id!;

                var (token, instanceUrl, error) = await AuthenticateAsync();
                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(instanceUrl))
                {
                    return Response.ErrorResponse("Authentication failed", error);
                }

                using var client = new HttpClient { BaseAddress = new Uri(instanceUrl) };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var body = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(acc.Name)) body["Name"] = acc.Name;
                if (!string.IsNullOrEmpty(acc.Email)) body["PersonEmail"] = acc.Email;
                if (!string.IsNullOrEmpty(acc.Address)) body["BillingStreet"] = acc.Address;

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"/services/data/v{_apiVersion}/sobjects/Account/{id}";
                var res = await client.PatchAsync(url, content);

                if (!res.IsSuccessStatusCode)
                {
                    var txt = await res.Content.ReadAsStringAsync();
                    return Response.ErrorResponse("Failed to update account", txt);
                }

                var output = new AccountOutput { Id = id, Name = acc.Name, Email = acc.Email, Address = acc.Address };
                return new Response { Success = true, Accounts = new List<AccountOutput> { output } };
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine(ex.ToString());
                return Response.FromException(ex);
            }
        }

        public async Task<Response> DeleteAccountAsync(Request request, ILambdaContext? context = null)
        {
            try
            {
                var id = request.Id ?? (request.Accounts != null && request.Accounts.Count > 0 ? request.Accounts[0].Id : null);
                if (string.IsNullOrEmpty(id))
                {
                    return Response.ErrorResponse("Id is required to delete account");
                }

                var (token, instanceUrl, error) = await AuthenticateAsync();
                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(instanceUrl))
                {
                    return Response.ErrorResponse("Authentication failed", error);
                }

                using var client = new HttpClient { BaseAddress = new Uri(instanceUrl) };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var url = $"/services/data/v{_apiVersion}/sobjects/Account/{id}";
                var res = await client.DeleteAsync(url);

                if (!res.IsSuccessStatusCode)
                {
                    var txt = await res.Content.ReadAsStringAsync();
                    return Response.ErrorResponse("Failed to delete account", txt);
                }

                return new Response { Success = true };
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine(ex.ToString());
                return Response.FromException(ex);
            }
        }
    }
}
