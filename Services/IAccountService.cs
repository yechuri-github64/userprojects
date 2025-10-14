using System.Collections.Generic;
using System.Threading.Tasks;
using accounts_management.Models;

namespace accounts_management.Services;

public interface IAccountService
{
    Task<List<Account>> GetAllAsync();
    Task<Account?> GetByIdAsync(int id);
    Task<Account> CreateAsync(Account account);
    Task<List<Account>> CreateManyAsync(IEnumerable<Account> accounts);
    Task<Account?> UpdateAsync(int id, Account updated);
    Task<bool> DeleteAsync(int id);
}
