using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using acc_sf_test.Models;

namespace acc_sf_test.Services
{
 public class AccountService : IAccountService
 {
 private readonly ISalesforceService _sf;
 private readonly SalesforceOptions _options;
 private readonly ILogger<AccountService> _logger;

 public AccountService(ISalesforceService sf, IOptions<SalesforceOptions> options, ILogger<AccountService> logger)
 {
 _sf = sf;
 _options = options.Value;
 _logger = logger;
 }

 private string ApiPath(string resource)
 {
 return $"/services/data/v{_options.ApiVersion}/sobjects/{resource}";
 }

 public async Task<IEnumerable<object>> CreateAccountsAsync(IEnumerable<AccountDto> accounts, CancellationToken ct = default)
 {
 var results = new List<object>();
 foreach (var acct in accounts)
 {
 try
 {
 var body = new Dictionary<string, object?>
 {
 { "Name", acct.Name },
 { "Phone", acct.Phone },
 { "Website", acct.Website },
 { "Type", acct.Type },
 { "Industry", acct.Industry }
 };

 var json = JsonSerializer.Serialize(body);
 var content = new StringContent(json, Encoding.UTF8, "application/json");
 var path = ApiPath("Account/");
 var resp = await _sf.SendApiAsync(HttpMethod.Post, path, content, ct);
 var respText = await resp.Content.ReadAsStringAsync(ct);
 if (resp.IsSuccessStatusCode)
 {
 using var doc = JsonDocument.Parse(respText);
 results.Add(new { success = true, response = doc.RootElement.Clone() });
 _logger.LogInformation("Created Account {Name} successfully", acct.Name);
 }
 else
 {
 _logger.LogWarning("Failed to create account {Name}. Status: {Status}. Response: {Response}", acct.Name, resp.StatusCode, respText);
 results.Add(new { success = false, status = resp.StatusCode.ToString(), response = respText });
 }
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Exception while creating account {Name}", acct.Name);
 results.Add(new { success = false, error = ex.Message });
 }
 }
 return results;
 }

 public async Task<object?> GetAccountAsync(string id, CancellationToken ct = default)
 {
 try
 {
 var path = ApiPath($"Account/{id}");
 var resp = await _sf.SendApiAsync(HttpMethod.Get, path, null, ct);
 var respText = await resp.Content.ReadAsStringAsync(ct);
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogWarning("GetAccount failed for {Id}. Status: {Status}. Response: {Response}", id, resp.StatusCode, respText);
 return new { success = false, status = resp.StatusCode.ToString(), response = respText };
 }
 using var doc = JsonDocument.Parse(respText);
 return doc.RootElement.Clone();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Exception while retrieving account {Id}", id);
 return new { success = false, error = ex.Message };
 }
 }

 public async Task<object?> UpdateAccountAsync(string id, AccountDto account, CancellationToken ct = default)
 {
 try
 {
 var body = new Dictionary<string, object?>();
 if (account.Name != null) body["Name"] = account.Name;
 if (account.Phone != null) body["Phone"] = account.Phone;
 if (account.Website != null) body["Website"] = account.Website;
 if (account.Type != null) body["Type"] = account.Type;
 if (account.Industry != null) body["Industry"] = account.Industry;

 var json = JsonSerializer.Serialize(body);
 var content = new StringContent(json, Encoding.UTF8, "application/json");
 var path = ApiPath($"Account/{id}");
 var resp = await _sf.SendApiAsync(HttpMethod.Patch, path, content, ct);
 if (resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.NoContent)
 {
 _logger.LogInformation("Updated Account {Id} successfully", id);
 return new { success = true };
 }
 var respText = await resp.Content.ReadAsStringAsync(ct);
 _logger.LogWarning("Failed to update account {Id}. Status: {Status}. Response: {Response}", id, resp.StatusCode, respText);
 return new { success = false, status = resp.StatusCode.ToString(), response = respText };
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Exception while updating account {Id}", id);
 return new { success = false, error = ex.Message };
 }
 }

 public async Task<bool> DeleteAccountAsync(string id, CancellationToken ct = default)
 {
 try
 {
 var path = ApiPath($"Account/{id}");
 var resp = await _sf.SendApiAsync(HttpMethod.Delete, path, null, ct);
 if (resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.NoContent)
 {
 _logger.LogInformation("Deleted Account {Id} successfully", id);
 return true;
 }
 var respText = await resp.Content.ReadAsStringAsync(ct);
 _logger.LogWarning("Failed to delete account {Id}. Status: {Status}. Response: {Response}", id, resp.StatusCode, respText);
 return false;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Exception while deleting account {Id}", id);
 return false;
 }
 }
 }
}