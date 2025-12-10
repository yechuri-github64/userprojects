using System.Collections.Generic;
using System.Threading.Tasks;
using account_c_sharp.Models;

namespace account_c_sharp.Services
{
 public interface IAccountService
 {
 Task<IEnumerable<Account>> GetAllAsync();
 Task<Account> GetByIdAsync(int id);
 Task<IEnumerable<Account>> CreateManyAsync(IEnumerable<Account> accounts);
 Task<bool> UpdateAsync(Account account);
 Task<bool> DeleteAsync(int id);
 }
}
