using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Salesforce.Common;
using Salesforce.Force;

namespace Services
{
 public class AccountService : IAccountService
 {
 private readonly ILogger<AccountService> _logger;
 private readonly IConfiguration _configuration;
 private readonly ConcurrentDictionary<string, Account> _mockStore = new();
 private readonly bool _useSalesforce;
 private ForceClient? _forceClient;

 public AccountService(ILogger<AccountService> logger, IConfiguration configuration)
 {
 _logger = logger;
 _configuration = configuration;

 var username = configuration["SalesforceUsername"];
 var password = configuration["SalesforcePassword"];
 var securityToken = configuration["SalesforceSecurityToken"];
 var clientId = configuration["ClientId"];
 var clientSecret = configuration["ClientSecret"];
 var loginUrl = configuration["SalesforceLoginUrl"];

 if (string.IsNullOrWhiteSpace(loginUrl))
 {
 loginUrl = "https://login.salesforce.com";
 }

 if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(securityToken) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
 {
 _logger.LogWarning("Salesforce credentials are not fully configured. Falling back to in-memory mock storage.");
 _useSalesforce = false;
 SeedMockData();
 return;
 }

 try
 {
 var auth = new AuthenticationClient();
 // UsernamePasswordAsync expects password+securityToken concatenated
 var pwdWithToken = password + securityToken;
 // Authenticate synchronously in constructor via GetAwaiter().GetResult() to avoid async ctor
 auth.UsernamePasswordAsync(clientId, clientSecret, username, pwdWithToken, loginUrl).GetAwaiter().GetResult();
 var apiVersion = "v58.0";
 if (!string.IsNullOrWhiteSpace(auth.ApiVersion)) apiVersion = auth.ApiVersion;
 _forceClient = new ForceClient(auth.InstanceUrl, auth.AccessToken, apiVersion);
 _useSalesforce = true;
 _logger.LogInformation("Authenticated to Salesforce instance: {Instance}", auth.InstanceUrl);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to authenticate to Salesforce. Falling back to in-memory mock storage.");
 _useSalesforce = false;
 SeedMockData();
 }
 }

 private void SeedMockData()
 {
 var a1 = new Account { Id = "001D000000IRFmaIAH", Name = "Acme Corp", Email = "info@acme.com", Address = "1 Acme Way" };
 var a2 = new Account { Id = "001D000000IRM02IAH", Name = "Contoso", Email = "hello@contoso.com", Address = "42 Contoso Drive" };
 var a3 = new Account { Id = "001D000000IRM1QIAW", Name = "Fabrikam", Email = "contact@fabrikam.com", Address = "7 Fabric St" };
 _mockStore[a1.Id] = a1;
 _mockStore[a2.Id] = a2;
 _mockStore[a3.Id] = a3;
 }

 public async Task<List<Account>> CreateAccountsAsync(List<Account> accounts)
 {
 var results = new List<Account>();
 if (accounts == null || accounts.Count == 0) return results;

 if (_useSalesforce && _forceClient != null)
 {
 foreach (var acc in accounts)
 {
 try
 {
 var sobj = new { Name = acc.Name, PersonEmail = acc.Email, BillingStreet = acc.Address };
 var createResult = await _forceClient.CreateAsync("Account", sobj);
 // createResult typically contains id in "id" or "Id"
 string id = string.Empty;
 try
 {
 var json = JsonSerializer.Serialize(createResult);
 using var doc = JsonDocument.Parse(json);
 if (doc.RootElement.TryGetProperty("id", out var idProp) || doc.RootElement.TryGetProperty("Id", out idProp))
 {
 id = idProp.GetString() ?? string.Empty;
 }
 }
 catch
 {
 // ignore parsing errors
 }

 if (string.IsNullOrWhiteSpace(id)) id = Guid.NewGuid().ToString();
 acc.Id = id;
 results.Add(acc);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error creating account {Name} in Salesforce", acc.Name);
 // continue with next and return partial results
 }
 }
 return results;
 }

 // Mock store
 foreach (var acc in accounts)
 {
 var id = string.IsNullOrWhiteSpace(acc.Id) ? Guid.NewGuid().ToString() : acc.Id;
 acc.Id = id;
 _mockStore[id] = acc;
 results.Add(acc);
 }

 await Task.CompletedTask;
 return results;
 }

 public async Task<Account?> GetAccountAsync(string id)
 {
 if (string.IsNullOrWhiteSpace(id)) return null;

 if (_useSalesforce && _forceClient != null)
 {
 try
 {
 var soql = $"SELECT Id, Name, PersonEmail, BillingStreet FROM Account WHERE Id = '{id}' LIMIT 1";
 var queryResult = await _forceClient.QueryAsync<dynamic>(soql);
 // queryResult.Records[0] expected
 try
 {
 var json = JsonSerializer.Serialize(queryResult);
 using var doc = JsonDocument.Parse(json);
 if (doc.RootElement.TryGetProperty("records", out var records) && records.GetArrayLength() > 0)
 {
 var rec = records[0];
 var account = new Account
 {
 Id = rec.GetProperty("Id").GetString() ?? string.Empty,
 Name = rec.TryGetProperty("Name", out var n) ? n.GetString() ?? string.Empty : string.Empty,
 Email = rec.TryGetProperty("PersonEmail", out var e) ? e.GetString() ?? string.Empty : string.Empty,
 Address = rec.TryGetProperty("BillingStreet", out var a) ? a.GetString() ?? string.Empty : string.Empty
 };
 return account;
 }
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error parsing Salesforce query result for id {Id}", id);
 }
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error querying Salesforce for account id {Id}", id);
 return null;
 }
 return null;
 }

 _mockStore.TryGetValue(id, out var account);
 await Task.CompletedTask;
 return account;
 }

 public async Task<Account?> UpdateAccountAsync(string id, Account account)
 {
 if (string.IsNullOrWhiteSpace(id) || account == null) return null;

 if (_useSalesforce && _forceClient != null)
 {
 try
 {
 var sobj = new { Name = account.Name, PersonEmail = account.Email, BillingStreet = account.Address };
 await _forceClient.UpdateAsync("Account", id, sobj);
 account.Id = id;
 return account;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error updating Salesforce account {Id}", id);
 return null;
 }
 }

 if (!_mockStore.ContainsKey(id)) return null;
 var existing = _mockStore[id];
 existing.Name = account.Name;
 existing.Email = account.Email;
 existing.Address = account.Address;
 await Task.CompletedTask;
 return existing;
 }

 public async Task<bool> DeleteAccountAsync(string id)
 {
 if (string.IsNullOrWhiteSpace(id)) return false;

 if (_useSalesforce && _forceClient != null)
 {
 try
 {
 await _forceClient.DeleteAsync("Account", id);
 return true;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error deleting Salesforce account {Id}", id);
 return false;
 }
 }

 var removed = _mockStore.TryRemove(id, out _);
 await Task.CompletedTask;
 return removed;
 }

 public async Task<List<Account>> ListAccountsAsync()
 {
 if (_useSalesforce && _forceClient != null)
 {
 try
 {
 var soql = "SELECT Id, Name, PersonEmail, BillingStreet FROM Account LIMIT 200";
 var queryResult = await _forceClient.QueryAsync<dynamic>(soql);
 var list = new List<Account>();
 try
 {
 var json = JsonSerializer.Serialize(queryResult);
 using var doc = JsonDocument.Parse(json);
 if (doc.RootElement.TryGetProperty("records", out var records))
 {
 foreach (var rec in records.EnumerateArray())
 {
 var account = new Account
 {
 Id = rec.TryGetProperty("Id", out var idp) ? idp.GetString() ?? string.Empty : string.Empty,
 Name = rec.TryGetProperty("Name", out var np) ? np.GetString() ?? string.Empty : string.Empty,
 Email = rec.TryGetProperty("PersonEmail", out var ep) ? ep.GetString() ?? string.Empty : string.Empty,
 Address = rec.TryGetProperty("BillingStreet", out var ap) ? ap.GetString() ?? string.Empty : string.Empty
 };
 list.Add(account);
 }
 }
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error parsing Salesforce list result");
 }

 return list;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error listing Salesforce accounts");
 return new List<Account>();
 }
 }

 var results = _mockStore.Values.ToList();
 await Task.CompletedTask;
 return results;
 }
 }
}