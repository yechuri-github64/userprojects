using System.Collections.Generic;
using System.Threading.Tasks;
using test-acc-sf-app.Models;

namespace test-acc-sf-app.Services
{
 public interface IAccountService
 {
 Task<IEnumerable<object>> CreateAccountsAsync(IEnumerable<AccountCreateRequest> requests);
 Task<AccountResponse> GetAccountAsync(string id);
 Task<bool> UpdateAccountAsync(string id, AccountUpdateRequest update);
 Task<bool> DeleteAccountAsync(string id);
 }
}
