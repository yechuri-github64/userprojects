using accounts_management.Data;
using accounts_management.Models;
using Microsoft.EntityFrameworkCore;

namespace accounts_management.Services
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
            return await _db.Accounts.AsNoTracking().ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Account> CreateAsync(Account account)
        {
            ValidateAccount(account);

            _db.Accounts.Add(account);
            await SaveChangesWithHandlingAsync("creating account");
            return account;
        }

        public async Task<IEnumerable<Account>> CreateBulkAsync(IEnumerable<Account> accounts)
        {
            var list = accounts.ToList();
            if (list.Count == 0)
                throw new ArgumentException("At least one account must be provided.");

            foreach (var a in list)
            {
                ValidateAccount(a);
            }

            var duplicateEmails = list.GroupBy(a => a.Email.Trim().ToLowerInvariant())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateEmails.Any())
            {
                throw new ArgumentException(
                    "Duplicate emails found in request: " + string.Join(", ", duplicateEmails)
                );
            }

            await _db.Accounts.AddRangeAsync(list);
            await SaveChangesWithHandlingAsync("creating accounts in bulk");
            return list;
        }

        public async Task<Account?> UpdateAsync(int id, Account account)
        {
            ValidateAccount(account);

            var existing = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (existing == null)
                return null;

            existing.Name = account.Name;
            existing.Email = account.Email;
            existing.Address = account.Address;

            await SaveChangesWithHandlingAsync($"updating account with id {id}");
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (existing == null)
                return false;

            _db.Accounts.Remove(existing);
            await SaveChangesWithHandlingAsync($"deleting account with id {id}");
            return true;
        }

        private static void ValidateAccount(Account account)
        {
            if (string.IsNullOrWhiteSpace(account.Name))
                throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(account.Email))
                throw new ArgumentException("Email is required.");
            if (account.Name.Length > 100)
                throw new ArgumentException("Name must be at most 100 characters.");
            if (account.Email.Length > 255)
                throw new ArgumentException("Email must be at most 255 characters.");
            if (account.Address != null && account.Address.Length > 500)
                throw new ArgumentException("Address must be at most 500 characters.");
        }

        private async Task SaveChangesWithHandlingAsync(string operation)
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update exception while {Operation}", operation);
                var message = "A database error occurred.";
                if (
                    ex.InnerException != null
                    && ex.InnerException.Message.Contains(
                        "Duplicate",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    message = "An account with the same email already exists.";
                }
                throw new InvalidOperationException(message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while {Operation}", operation);
                throw;
            }
        }
    }
}
