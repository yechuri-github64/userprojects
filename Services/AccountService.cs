using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Salesforce.Common.Models.Json;
using Salesforce.Common;
using Salesforce.Force;
using Models;

namespace Services
{
 public class AccountService : IAccountService
 {
 private readonly ForceClient _forceClient;
 private readonly ILogger<AccountService> _logger;
 private const string ApiVersion = "v58.0";

 public AccountService(IConfiguration configuration, ILogger<AccountService> logger)
 {
 _logger = logger;
 try
 {
 var username = configuration["SalesforceUsername"] ?? string.Empty;
 var password = configuration["SalesforcePassword"] ?? string.Empty;
 var securityToken = configuration["SalesforceSecurityToken"] ?? string.Empty;
 var clientId = configuration["ClientId"] ?? string.Empty;
 var clientSecret = configuration["ClientSecret"] ?? string.Empty;
 var loginUrl = configuration["SalesforceLoginUrl"];
 var useSandbox = false;
 if (bool.TryParse(configuration["UseSandbox"], out var parsed)) useSandbox = parsed;
 if (string.IsNullOrWhiteSpace(loginUrl)) loginUrl = useSandbox ? "https://test.salesforce.com" : "https://login.salesforce.com";

 if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
 {
 _logger.LogWarning("Salesforce credentials are not fully configured. AccountService will not initialize ForceClient.");
 _forceClient = null!; // will cause errors if called without proper config
 return;
 }

 var auth = new AuthenticationClient();
 // UsernamePasswordAsync expects password + security token concatenated
 var fullPassword = password + (securityToken ?? string.Empty);
 auth.UsernamePasswordAsync(clientId, clientSecret, username, fullPassword, loginUrl).GetAwaiter().GetResult();

 var instanceUrl = auth.InstanceUrl;
 var accessToken = auth.AccessToken;
 _forceClient = new ForceClient(instanceUrl, accessToken, ApiVersion);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to initialize Salesforce ForceClient.");
 throw;
 }
 }

 public async Task<List<Account>> CreateAccountsAsync(List<Account> accounts)
 {
 if (_forceClient == null) throw new InvalidOperationException("Salesforce client is not initialized.");
 var created = new List<Account>();
 foreach (var acct in accounts)
 {
 try
 {
 var sObj = new
 {
 Name = acct.Name,
 PersonEmail = string.IsNullOrWhiteSpace(acct.Email) ? null : acct.Email,
 BillingStreet = string.IsNullOrWhiteSpace(acct.Address) ? null : acct.Address
 };

 dynamic result = await _forceClient.CreateAsync("Account", sObj);
 string id = (result?.id ?? result?.Id)?.ToString() ?? string.Empty;
 var outAcct = new Account
 {
 Id = id,
 Name = acct.Name,
 Email = acct.Email,
 Address = acct.Address
 };
 created.Add(outAcct);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error creating account: {Name}", acct.Name);
 throw;
 }
 }
 return created;
 }

 public async Task<Account?> GetAccountAsync(string id)
 {
 if (_forceClient == null) throw new InvalidOperationException("Salesforce client is not initialized.");
 try
 {
 // Use sobjects API to get fields
 dynamic rec = await _forceClient.GetAsync<dynamic>($"sobjects/Account/{id}");
 if (rec == null) return null;
 var account = new Account
 {
 Id = id,
 Name = rec.Name ?? string.Empty,
 Email = rec.PersonEmail ?? string.Empty,
 Address = rec.BillingStreet ?? string.Empty
 };
 return account;
 }
 catch (ForceException fex) when (fex.Message.Contains("NOT_FOUND") || fex.Message.Contains("not found"))
 {
 return null;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error retrieving account {Id}", id);
 throw;
 }
 }

 public async Task<Account?> UpdateAccountAsync(string id, Account account)
 {
 if (_forceClient == null) throw new InvalidOperationException("Salesforce client is not initialized.");
 try
 {
 var sObj = new
 {
 Name = account.Name,
 PersonEmail = string.IsNullOrWhiteSpace(account.Email) ? null : account.Email,
 BillingStreet = string.IsNullOrWhiteSpace(account.Address) ? null : account.Address
 };

 await _forceClient.UpdateAsync("Account", id, sObj);

 var updated = await GetAccountAsync(id);
 return updated;
 }
 catch (ForceException fex) when (fex.Message.Contains("NOT_FOUND") || fex.Message.Contains("not found"))
 {
 return null;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error updating account {Id}", id);
 throw;
 }
 }

 public async Task<bool> DeleteAccountAsync(string id)
 {
 if (_forceClient == null) throw new InvalidOperationException("Salesforce client is not initialized.");
 try
 {
 await _forceClient.DeleteAsync("Account", id);
 return true;
 }
 catch (ForceException fex) when (fex.Message.Contains("NOT_FOUND") || fex.Message.Contains("not found"))
 {
 return false;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error deleting account {Id}", id);
 throw;
 }
 }

 public async Task<List<Account>> ListAccountsAsync()
 {
 if (_forceClient == null) throw new InvalidOperationException("Salesforce client is not initialized.");
 try
 {
 var q = "SELECT Id, Name, PersonEmail, BillingStreet FROM Account LIMIT 200";
 var queryResult = await _forceClient.QueryAsync<dynamic>(q);
 var records = new List<Account>();
 if (queryResult?.records != null)
 {
 foreach (var r in queryResult.records)
 {
 records.Add(new Account
 {
 Id = r.Id ?? string.Empty,
 Name = r.Name ?? string.Empty,
 Email = r.PersonEmail ?? string.Empty,
 Address = r.BillingStreet ?? string.Empty
 });
 }
 }
 return records;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error listing accounts");
 throw;
 }
 }
 }
}