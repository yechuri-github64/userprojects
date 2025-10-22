using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using acc_sf_test.Models;
using acc_sf_test.Data;

namespace acc_sf_test.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _db;
        private readonly SalesforceClient _sfClient;
        private readonly ILogger<AccountService> _logger;

        public AccountService(ApplicationDbContext db, SalesforceClient sfClient, ILogger<AccountService> logger)
        {
            _db = db;
            _sfClient = sfClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _db.Accounts.AsNoTracking().ToListAsync();
        }

        public async Task<Account> GetByIdAsync(Guid id)
        {
            return await _db.Accounts.FindAsync(id);
        }

        public async Task<IEnumerable<Account>> CreateAsync(IEnumerable<AccountRequest> requests)
        {
            if (requests == null || !requests.Any()) throw new ArgumentException("At least one account is required to create.");

            var created = new List<Account>();

            foreach (var req in requests)
            {
                if (string.IsNullOrWhiteSpace(req.Name)) throw new ArgumentException("Account name is required.");

                // Call Salesforce to create the account
                string sfId = null;
                try
                {
                    sfId = await _sfClient.CreateAccountAsync(req);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Salesforce create failed for account {Name}", req.Name);
                    throw; // bubble up to controller
                }

                var entity = new Account
                {
                    Id = Guid.NewGuid(),
                    Name = req.Name,
                    Industry = req.Industry,
                    Phone = req.Phone,
                    Website = req.Website,
                    SalesforceId = sfId
                };

                _db.Accounts.Add(entity);
                created.Add(entity);
            }

            await _db.SaveChangesAsync();
            return created;
        }

        public async Task<Account> UpdateAsync(Guid id, AccountRequest request)
        {
            var existing = await _db.Accounts.FindAsync(id);
            if (existing == null) return null;

            if (!string.IsNullOrWhiteSpace(request.Name)) existing.Name = request.Name;
            existing.Industry = request.Industry;
            existing.Phone = request.Phone;
            existing.Website = request.Website;

            // Update in Salesforce if SalesforceId is present
            if (!string.IsNullOrWhiteSpace(existing.SalesforceId))
            {
                try
                {
                    await _sfClient.UpdateAccountAsync(existing.SalesforceId, request);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Salesforce update failed for account id {Id} sfId {SfId}", id, existing.SalesforceId);
                    throw;
                }
            }

            _db.Accounts.Update(existing);
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _db.Accounts.FindAsync(id);
            if (existing == null) return false;

            // Delete in Salesforce if present
            if (!string.IsNullOrWhiteSpace(existing.SalesforceId))
            {
                try
                {
                    await _sfClient.DeleteAccountAsync(existing.SalesforceId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Salesforce delete failed for account id {Id} sfId {SfId}", id, existing.SalesforceId);
                    throw;
                }
            }

            _db.Accounts.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
