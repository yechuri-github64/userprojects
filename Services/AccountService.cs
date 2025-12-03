using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;

namespace Services
{
 public class AccountService : IAccountService
 {
        private readonly ILogger<AccountService> _logger;
        private Dictionary<string, Account> _mockAccounts = new(); public AccountService(IConfiguration configuration, ILogger<AccountService> logger)
 {
 _logger = logger;
 try
 {
 var username = configuration["SalesforceUsername"] ?? string.Empty;
 var password = configuration["SalesforcePassword"] ?? string.Empty;
 var clientId = configuration["ClientId"] ?? string.Empty;
 var clientSecret = configuration["ClientSecret"] ?? string.Empty;

 if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
 {
 _logger.LogWarning("Salesforce credentials are not fully configured. Using mock data.");
 InitializeMockData();
 return;
 }

 _logger.LogInformation("AccountService initialized for user: {Username}", username);
 InitializeMockData();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to initialize AccountService.");
 throw;
 }
 }

 private void InitializeMockData()
 {
 _mockAccounts = new Dictionary<string, Account>
 {
 {
 "001D000000IRFmaIAH", new Account
 {
 Id = "001D000000IRFmaIAH",
 Name = "Sample Account 1",
 Email = "contact@sample1.com",
 Address = "123 Main Street, San Francisco, CA 94105"
 }
 },
 {
 "001D000000IRM02IAH", new Account
 {
 Id = "001D000000IRM02IAH",
 Name = "Sample Account 2",
 Email = "contact@sample2.com",
 Address = "456 Oak Avenue, New York, NY 10001"
 }
 },
 {
 "001D000000IRM1QIAW", new Account
 {
 Id = "001D000000IRM1QIAW",
 Name = "Sample Account 3",
 Email = "contact@sample3.com",
 Address = "789 Pine Road, Austin, TX 78701"
 }
 }
 };
 }

 public async Task<List<Account>> CreateAccountsAsync(List<Account> accounts)
 {
 var created = new List<Account>();
 foreach (var acct in accounts)
 {
 try
 {
 var newId = Guid.NewGuid().ToString().Substring(0, 18).ToUpper();
 var newAccount = new Account
 {
 Id = newId,
 Name = acct.Name,
 Email = acct.Email,
 Address = acct.Address
 };
 _mockAccounts[newId] = newAccount;
 created.Add(newAccount);
 _logger.LogInformation("Created account: {AccountId} - {AccountName}", newId, acct.Name);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error creating account: {Name}", acct.Name);
 throw;
 }
 }
 return await Task.FromResult(created);
 }

 public async Task<Account?> GetAccountAsync(string id)
 {
 try
 {
 if (_mockAccounts.TryGetValue(id, out var account))
 {
 _logger.LogInformation("Retrieved account: {AccountId}", id);
 return await Task.FromResult(account);
 }
 _logger.LogWarning("Account not found: {AccountId}", id);
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
 try
 {
 if (!_mockAccounts.ContainsKey(id))
 {
 _logger.LogWarning("Account not found for update: {AccountId}", id);
 return null;
 }

 var updated = new Account
 {
 Id = id,
 Name = account.Name,
 Email = account.Email,
 Address = account.Address
 };
 _mockAccounts[id] = updated;
 _logger.LogInformation("Updated account: {AccountId}", id);
 return await Task.FromResult(updated);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error updating account {Id}", id);
 throw;
 }
 }

 public async Task<bool> DeleteAccountAsync(string id)
 {
 try
 {
 if (_mockAccounts.ContainsKey(id))
 {
 _mockAccounts.Remove(id);
 _logger.LogInformation("Deleted account: {AccountId}", id);
 return await Task.FromResult(true);
 }
 _logger.LogWarning("Account not found for deletion: {AccountId}", id);
 return await Task.FromResult(false);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error deleting account {Id}", id);
 throw;
 }
 }

 public async Task<List<Account>> ListAccountsAsync()
 {
 try
 {
 var records = _mockAccounts.Values.ToList();
 _logger.LogInformation("Listed {Count} accounts", records.Count);
 return await Task.FromResult(records);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error listing accounts");
 throw;
 }
 }
 }
}