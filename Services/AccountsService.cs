using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using RestSharp;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Services
{
 public class AccountsService : IAccountsService
 {
 private readonly RestClient _client;
 private readonly string _apiKey;
 private readonly ILogger<AccountsService> _logger;

 public AccountsService(RestClient client, string apiKey, ILogger<AccountsService> logger)
 {
 _client = client;
 _apiKey = apiKey ?? string.Empty;
 _logger = logger;
 }

 private RestRequest CreateRequest(string resource, Method method)
 {
 var req = new RestRequest(resource, method);
 if (!string.IsNullOrEmpty(_apiKey))
 {
 req.AddHeader("x-api-key", _apiKey);
 }
 req.AddHeader("Accept", "application/json");
 return req;
 }

 public async Task<IEnumerable<Account>> GetAllAsync()
 {
 var req = CreateRequest("accounts", Method.Get);
 var resp = await _client.ExecuteAsync(req);
 if (!resp.IsSuccessful)
 {
 _logger.LogError("Failed to get accounts. Status: {Status}, Content: {Content}", resp.StatusCode, resp.Content);
 throw new ApplicationException($"Failed to get accounts: {resp.StatusCode}");
 }
 return JsonSerializer.Deserialize<IEnumerable<Account>>(resp.Content) ?? new List<Account>();
 }

 public async Task<Account?> GetByIdAsync(string id)
 {
 var req = CreateRequest($"accounts/{id}", Method.Get);
 var resp = await _client.ExecuteAsync(req);
 if (!resp.IsSuccessful)
 {
 if ((int)resp.StatusCode == 404) return null;
 _logger.LogError("Failed to get account {Id}. Status: {Status}, Content: {Content}", id, resp.StatusCode, resp.Content);
 throw new ApplicationException($"Failed to get account {id}: {resp.StatusCode}");
 }
 return JsonSerializer.Deserialize<Account>(resp.Content);
 }

 public async Task<Account> CreateAsync(Account account)
 {
 var req = CreateRequest("accounts", Method.Post);
 req.AddJsonBody(account);
 var resp = await _client.ExecuteAsync(req);
 if (!resp.IsSuccessful)
 {
 _logger.LogError("Failed to create account. Status: {Status}, Content: {Content}", resp.StatusCode, resp.Content);
 throw new ApplicationException($"Failed to create account: {resp.StatusCode}");
 }
 return JsonSerializer.Deserialize<Account>(resp.Content);
 }

 public async Task<IEnumerable<Account>> CreateBatchAsync(IEnumerable<Account> accounts)
 {
 var req = CreateRequest("accounts/batch", Method.Post);
 req.AddJsonBody(new { accounts });
 var resp = await _client.ExecuteAsync(req);
 if (!resp.IsSuccessful)
 {
 _logger.LogError("Failed to create accounts batch. Status: {Status}, Content: {Content}", resp.StatusCode, resp.Content);
 throw new ApplicationException($"Failed to create accounts batch: {resp.StatusCode}");
 }
 return JsonSerializer.Deserialize<IEnumerable<Account>>(resp.Content) ?? new List<Account>();
 }

 public async Task<Account?> UpdateAsync(string id, Account update)
 {
 var req = CreateRequest($"accounts/{id}", Method.Put);
 req.AddJsonBody(update);
 var resp = await _client.ExecuteAsync(req);
 if (!resp.IsSuccessful)
 {
 if ((int)resp.StatusCode == 404) return null;
 _logger.LogError("Failed to update account {Id}. Status: {Status}, Content: {Content}", id, resp.StatusCode, resp.Content);
 throw new ApplicationException($"Failed to update account {id}: {resp.StatusCode}");
 }
 return JsonSerializer.Deserialize<Account>(resp.Content);
 }

 public async Task<bool> DeleteAsync(string id)
 {
 var req = CreateRequest($"accounts/{id}", Method.Delete);
 var resp = await _client.ExecuteAsync(req);
 if (!resp.IsSuccessful)
 {
 if ((int)resp.StatusCode == 404) return false;
 _logger.LogError("Failed to delete account {Id}. Status: {Status}, Content: {Content}", id, resp.StatusCode, resp.Content);
 throw new ApplicationException($"Failed to delete account {id}: {resp.StatusCode}");
 }
 return true;
 }
 }
}
