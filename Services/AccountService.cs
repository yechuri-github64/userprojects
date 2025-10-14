using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using accounts_management.Data;
using accounts_management.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace accounts_management.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<AccountService> _logger;

    public AccountService(ApplicationDbContext db, ILogger<AccountService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<Account>> GetAllAsync()
    {
        return await _db.Accounts.AsNoTracking().ToListAsync();
    }

    public async Task<Account?> GetByIdAsync(int id)
    {
        return await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Account> CreateAsync(Account account)
    {
        try
        {
            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();
            return account;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error creating account with email {Email}", account.Email);
            throw;
        }
    }

    public async Task<List<Account>> CreateManyAsync(IEnumerable<Account> accounts)
    {
        try
        {
            var list = accounts.ToList();
            await _db.Accounts.AddRangeAsync(list);
            await _db.SaveChangesAsync();
            return list;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error creating multiple accounts");
            throw;
        }
    }

    public async Task<Account?> UpdateAsync(int id, Account updated)
    {
        var existing = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        if (existing == null)
        {
            return null;
        }
        existing.Name = updated.Name;
        existing.Email = updated.Email;
        existing.Address = updated.Address;
        try
        {
            await _db.SaveChangesAsync();
            return existing;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error updating account with id {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        if (existing == null)
        {
            return false;
        }
        _db.Accounts.Remove(existing);
        try
        {
            await _db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error deleting account with id {Id}", id);
            throw;
        }
    }
}
