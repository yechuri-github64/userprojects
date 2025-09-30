using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using accounts_operation.Data;
using accounts_operation.Models;

namespace accounts_operation.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Accounts.AsNoTracking().ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Account> CreateAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<bool> UpdateAsync(Account account)
        {
            var exists = await _context.Accounts.AnyAsync(a => a.Id == account.Id);
            if (!exists) return false;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.Accounts.FindAsync(id);
            if (item == null) return false;
            _context.Accounts.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
