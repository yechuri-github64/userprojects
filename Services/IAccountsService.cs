using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Services
{
 public interface IAccountsService
 {
 Task<IEnumerable<Account>> GetAllAsync();
 Task<Account?> GetByIdAsync(string id);
 Task<Account> CreateAsync(Account account);
 Task<IEnumerable<Account>> CreateBatchAsync(IEnumerable<Account> accounts);
 Task<Account?> UpdateAsync(string id, Account update);
 Task<bool> DeleteAsync(string id);
 }
}
