using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using test-acc-sf-app.Models;

namespace test-acc-sf-app.Services
{
 public class AccountService : IAccountService
 {
 private readonly ISalesforceClient _sfClient;
 private readonly ILogger<AccountService> _logger;

 public AccountService(ISalesforceClient sfClient, ILogger<AccountService> logger)
 {
 _sfClient = sfClient;
 _logger = logger;
 }

 public async Task<IEnumerable<object>> CreateAccountsAsync(IEnumerable<AccountCreateRequest> requests)
 {
 var created = new List<object>();
 foreach (var req in requests)
 {
 try
 {
 var payload = new Dictionary<string, object>();
 if (!string.IsNullOrWhiteSpace(req.Name)) payload["Name"] = req.Name;
 if (!string.IsNullOrWhiteSpace(req.Phone)) payload["Phone"] = req.Phone;
 if (!string.IsNullOrWhiteSpace(req.Website)) payload["Website"] = req.Website;

 var result = await _sfClient.CreateAsync("sobjects/Account", payload);
 created.Add(result);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to create one of the accounts");
 created.Add(new { error = "failed", message = ex.Message });
 }
 }

 return created;
 }

 public async Task<AccountResponse> GetAccountAsync(string id)
 {
 try
 {
 var fields = "Id,Name,Phone,Website";
 var response = await _sfClient.GetAsync($"sobjects/Account/{id}?fields={fields}");
 if (response == null) return null;

 using var doc = JsonDocument.Parse(response);
 var root = doc.RootElement;

 if (root.TryGetProperty("error", out _)) return null;

 var account = new AccountResponse
 {
 Id = root.GetProperty("Id").GetString(),
 Name = root.TryGetProperty("Name", out var n) ? n.GetString() : null,
 Phone = root.TryGetProperty("Phone", out var p) ? p.GetString() : null,
 Website = root.TryGetProperty("Website", out var w) ? w.GetString() : null
 };

 return account;
 }
 catch (HttpRequestException ex)
 {
 _logger.LogError(ex, "HTTP error when getting account {AccountId}", id);
 throw;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Unexpected error when getting account {AccountId}", id);
 throw;
 }
 }

 public async Task<bool> UpdateAccountAsync(string id, AccountUpdateRequest update)
 {
 try
 {
 var payload = new Dictionary<string, object>();
 if (update.Name != null) payload["Name"] = update.Name;
 if (update.Phone != null) payload["Phone"] = update.Phone;
 if (update.Website != null) payload["Website"] = update.Website;

 var result = await _sfClient.PatchAsync($"sobjects/Account/{id}", payload);
 return result;
 }
 catch (HttpRequestException ex)
 {
 _logger.LogError(ex, "HTTP error when updating account {AccountId}", id);
 throw;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Unexpected error when updating account {AccountId}", id);
 throw;
 }
 }

 public async Task<bool> DeleteAccountAsync(string id)
 {
 try
 {
 var result = await _sfClient.DeleteAsync($"sobjects/Account/{id}");
 return result;
 }
 catch (HttpRequestException ex)
 {
 _logger.LogError(ex, "HTTP error when deleting account {AccountId}", id);
 throw;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Unexpected error when deleting account {AccountId}", id);
 throw;
 }
 }
 }
}
