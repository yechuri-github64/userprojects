using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using account_c_sharp.Models;

namespace account_c_sharp.Services
{
 public class AccountService : IAccountService
 {
 private readonly ApplicationDbContext _db;
 private readonly ILogger<AccountService> _logger;

 public AccountService(ApplicationDbContext db, ILogger<AccountService> logger)
 {
 _db = db;
 _logger = logger;
 }

 public async Task<IEnumerable<Account>> GetAllAsync()
 {
 try
 {
 return await _db.GetAllAsync();
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
 return await _db.GetByIdAsync(id);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to get account by id: {Id}", id);
 throw;
 }
 }

 public async Task<IEnumerable<Account>> CreateManyAsync(IEnumerable<Account> accounts)
 {
 if (accounts == null) throw new ArgumentException("Accounts collection cannot be null");

 try
 {
 return await _db.CreateManyAsync(accounts);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to create accounts in bulk");
 throw;
 }
 }

 public async Task<bool> UpdateAsync(Account account)
 {
 if (account == null) throw new ArgumentException("Account cannot be null");
 try
 {
 return await _db.UpdateAsync(account);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to update account id: {Id}", account.Id);
 throw;
 }
 }

 public async Task<bool> DeleteAsync(int id)
 {
 try
 {
 return await _db.DeleteAsync(id);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to delete account id: {Id}", id);
 throw;
 }
 }
 }
}
