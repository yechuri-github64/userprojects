using System.Collections.Generic;
using System.Threading.Tasks;
using accounts_management_c_sharp.Models;

namespace accounts_management_c_sharp.Services
{
 public interface IAccountService
 {
 Task<IEnumerable<Account>> GetAllAsync();
 Task<Account> GetByIdAsync(int id);
 Task<IEnumerable<Account>> CreateManyAsync(IEnumerable<Account> accounts);
 Task<bool> UpdateAsync(int id, Account account);
 Task<bool> DeleteAsync(int id);
 }
}
