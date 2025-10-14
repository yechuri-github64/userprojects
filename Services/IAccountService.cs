using accounts_management.Models;

namespace accounts_management.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task<Account> CreateAsync(Account account);
        Task<IEnumerable<Account>> CreateBulkAsync(IEnumerable<Account> accounts);
        Task<Account?> UpdateAsync(int id, Account account);
        Task<bool> DeleteAsync(int id);
    }
}
