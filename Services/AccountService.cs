using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Salesforce.Common;
using Salesforce.Force;

namespace Services
{
 public class AccountService : IAccountService
 {
 private readonly object _authLock = new object();
 private AuthenticationClient? _authClient;
 private ForceClient? _forceClient;
 private string _apiVersion = "v58.0";

 private async Task EnsureAuthenticatedAsync()
 {
 if (_forceClient != null) return;
 lock (_authLock)
 {
 if (_forceClient != null) return;
 }

 var username = Environment.GetEnvironmentVariable("SalesforceUsername") ?? string.Empty;
 var password = Environment.GetEnvironmentVariable("SalesforcePassword") ?? string.Empty;
 var securityToken = Environment.GetEnvironmentVariable("SalesforceSecurityToken") ?? string.Empty;
 var loginUrl = Environment.GetEnvironmentVariable("SalesforceLoginUrl") ?? "https://login.salesforce.com";
 var clientId = Environment.GetEnvironmentVariable("ClientId") ?? string.Empty;
 var clientSecret = Environment.GetEnvironmentVariable("ClientSecret") ?? string.Empty;
 var useSandbox = (Environment.GetEnvironmentVariable("UseSandbox") ?? "false").ToLower() == "true";

 if (useSandbox)
 {
 loginUrl = Environment.GetEnvironmentVariable("SalesforceLoginUrl") ?? "https://test.salesforce.com";
 }

 if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
 {
 throw new InvalidOperationException("Salesforce credentials or client configuration are not set in environment variables.");
 }

 _authClient = new AuthenticationClient();
 try
 {
 // UsernamePasswordAsync(clientId, clientSecret, username, password + securityToken, loginUrl)
 await _authClient.UsernamePasswordAsync(clientId, clientSecret, username, password + securityToken, loginUrl).ConfigureAwait(false);
 var instanceUrl = _authClient.InstanceUrl ?? Environment.GetEnvironmentVariable("InstanceUrl") ?? string.Empty;
 var accessToken = _authClient.AccessToken ?? throw new InvalidOperationException("Failed to retrieve access token from Salesforce.");
 _forceClient = new ForceClient(instanceUrl, accessToken, _apiVersion);
 }
 catch (Exception ex)
 {
 throw new InvalidOperationException("Salesforce authentication failed: " + ex.Message, ex);
 }
 }

 public async Task<List<Account>> CreateAccountsAsync(List<Account> accounts)
 {
 if (accounts == null) throw new ArgumentNullException(nameof(accounts));
 await EnsureAuthenticatedAsync().ConfigureAwait(false);

 var createdAccounts = new List<Account>();
 foreach (var account in accounts)
 {
 try
 {
 var sobject = new Dictionary<string, object?>
 {
 { "Name", account.Name },
 { "PersonEmail", string.IsNullOrWhiteSpace(account.Email) ? null : account.Email },
 { "BillingStreet", string.IsNullOrWhiteSpace(account.Address) ? null : account.Address }
 };

 var result = await _forceClient!.CreateAsync("Account", sobject).ConfigureAwait(false);
 // result should contain id and success
 if (result is Newtonsoft.Json.Linq.JObject jObj && jObj["id"] != null)
 {
 account.Id = jObj["id"].ToString()!;
 }
 else if (result is dynamic d && d.id != null)
 {
 account.Id = d.id;
 }
 else if (result is IDictionary<string, object> dict && dict.ContainsKey("id"))
 {
 account.Id = dict["id"]?.ToString() ?? string.Empty;
 }
 else
 {
 // Fallback: generate id
 account.Id = Guid.NewGuid().ToString();
 }

 createdAccounts.Add(account);
 }
 catch (Exception ex)
 {
 // If creation of one account fails, include it with Id empty and log error in its Email field as indicator
 createdAccounts.Add(new Account { Id = string.Empty, Name = account.Name, Email = $"ERROR: {ex.Message}", Address = account.Address });
 }
 }

 return createdAccounts;
 }

 public async Task<Account?> GetAccountAsync(string id)
 {
 if (string.IsNullOrWhiteSpace(id)) return null;
 await EnsureAuthenticatedAsync().ConfigureAwait(false);

 try
 {
 var obj = await _forceClient!.GetAsync<Newtonsoft.Json.Linq.JObject>("Account", id).ConfigureAwait(false);
 if (obj == null) return null;
 var account = new Account
 {
 Id = obj.Value<string>("Id") ?? string.Empty,
 Name = obj.Value<string>("Name") ?? string.Empty,
 Email = obj.Value<string>("PersonEmail") ?? string.Empty,
 Address = obj.Value<string>("BillingStreet") ?? string.Empty
 };
 return account;
 }
 catch (ForceException fx) when (fx?.Error != null)
 {
 return null;
 }
 catch
 {
 throw;
 }
 }

 public async Task<Account?> UpdateAccountAsync(string id, Account account)
 {
 if (string.IsNullOrWhiteSpace(id) || account == null) return null;
 await EnsureAuthenticatedAsync().ConfigureAwait(false);

 try
 {
 var sobject = new Dictionary<string, object?>();
 if (!string.IsNullOrWhiteSpace(account.Name)) sobject["Name"] = account.Name;
 if (!string.IsNullOrWhiteSpace(account.Email)) sobject["PersonEmail"] = account.Email;
 if (!string.IsNullOrWhiteSpace(account.Address)) sobject["BillingStreet"] = account.Address;

 await _forceClient!.UpdateAsync("Account", id, sobject).ConfigureAwait(false);

 var updated = await GetAccountAsync(id).ConfigureAwait(false);
 return updated;
 }
 catch (Exception)
 {
 return null;
 }
 }

 public async Task<bool> DeleteAccountAsync(string id)
 {
 if (string.IsNullOrWhiteSpace(id)) return false;
 await EnsureAuthenticatedAsync().ConfigureAwait(false);

 try
 {
 await _forceClient!.DeleteAsync("Account", id).ConfigureAwait(false);
 return true;
 }
 catch
 {
 return false;
 }
 }

 public async Task<List<Account>> ListAccountsAsync()
 {
 await EnsureAuthenticatedAsync().ConfigureAwait(false);

 try
 {
 var soql = "SELECT Id, Name, PersonEmail, BillingStreet FROM Account LIMIT 200";
 var queryResult = await _forceClient!.QueryAsync<Newtonsoft.Json.Linq.JObject>(soql).ConfigureAwait(false);
 var list = new List<Account>();
 if (queryResult == null) return list;
 var records = queryResult["records"] as Newtonsoft.Json.Linq.JArray;
 if (records == null) return list;
 foreach (var rec in records)
 {
 var id = rec.Value<string>("Id") ?? string.Empty;
 var name = rec.Value<string>("Name") ?? string.Empty;
 var email = rec.Value<string>("PersonEmail") ?? string.Empty;
 var street = rec.Value<string>("BillingStreet") ?? string.Empty;
 list.Add(new Account { Id = id, Name = name, Email = email, Address = street });
 }

 return list;
 }
 catch (Exception ex)
 {
 throw new InvalidOperationException("Failed to list accounts: " + ex.Message, ex);
 }
 }
 }
}