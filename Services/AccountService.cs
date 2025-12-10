using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using account-mangement.Models;

namespace account-mangement.Services
{
 public class AccountService : IAccountService
 {
 private readonly AccountRepository _repository;
 private readonly ILogger<AccountService> _logger;

 public AccountService(AccountRepository repository, ILogger<AccountService> logger)
 {
 _repository = repository;
 _logger = logger;
 }

 public async Task<IEnumerable<Account>> GetAllAsync()
 {
 try
 {
 return await _repository.GetAllAsync();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to get all accounts");
 throw;
 }
 }

 public async Task<Account> GetByIdAsync(int id)
 {
 try
 {
 return await _repository.GetByIdAsync(id);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to get account {Id}", id);
 throw;
 }
 }

 public async Task<IEnumerable<Account>> CreateMultipleAsync(IEnumerable<Account> accounts)
 {
 if (accounts == null) throw new ArgumentException("Accounts collection cannot be null.");

 try
 {
 return await _repository.CreateMultipleAsync(accounts);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to create accounts");
 throw;
 }
 }

 public async Task<bool> UpdateAsync(Account account)
 {
 if (account == null) throw new ArgumentException("Account cannot be null.");
 if (account.Id <= 0) throw new ArgumentException("Account must have a valid id.");

 try
 {
 return await _repository.UpdateAsync(account);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to update account {Id}", account.Id);
 throw;
 }
 }
 }
}
