using System.Collections.Generic;
using System.Threading.Tasks;
using account-mangement.Models;

namespace account-mangement.Services
{
 public interface IAccountService
 {
 Task<IEnumerable<Account>> GetAllAsync();
 Task<Account> GetByIdAsync(int id);
 Task<IEnumerable<Account>> CreateMultipleAsync(IEnumerable<Account> accounts);
 Task<bool> UpdateAsync(Account account);
 }
}
