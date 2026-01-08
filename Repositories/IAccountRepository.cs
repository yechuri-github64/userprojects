using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManagerFunctionApp.Models;

namespace AccountManagerFunctionApp.Repositories
{
 public interface IAccountRepository
 {
 Task<IEnumerable<Account>> GetAllAsync();
 Task<Account?> GetByIdAsync(Guid id);
 Task<IEnumerable<Account>> CreateBatchAsync(IEnumerable<Account> accounts);
 Task<Account?> UpdateAsync(Guid id, Account updated);
 Task<bool> DeleteAsync(Guid id);
 }
}
