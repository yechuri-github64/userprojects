using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
 public interface IAccountService
 {
 Task<List<Account>> CreateAccountsAsync(List<Account> accounts);
 Task<Account?> GetAccountAsync(string id);
 Task<Account?> UpdateAccountAsync(string id, Account account);
 Task<bool> DeleteAccountAsync(string id);
 Task<List<Account>> ListAccountsAsync();
 }
}