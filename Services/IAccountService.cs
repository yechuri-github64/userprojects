using System.Collections.Generic;
using System.Threading.Tasks;
using accounts_operation.Models;

namespace accounts_operation.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task<Account> CreateAsync(Account account);
        Task<bool> UpdateAsync(Account account);
        Task<bool> DeleteAsync(int id);
    }
}
