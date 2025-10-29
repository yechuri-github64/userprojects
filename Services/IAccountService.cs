using System.Collections.Generic;
using System.Threading.Tasks;
using acc-sf-test.Models;

namespace acc-sf-test.Services
{
 public interface IAccountService
 {
 Task<IEnumerable<object>> CreateAccountsAsync(IEnumerable<AccountRequest> requests);
 Task<object?> GetAccountAsync(string id);
 Task<object> UpdateAccountAsync(string id, AccountRequest request);
 Task DeleteAccountAsync(string id);
 }
}
