using System.Collections.Generic;
using System.Threading.Tasks;
using acc-sf-test.Models;

namespace acc-sf-test.Repositories
{
 public interface ISalesforceRepository
 {
 Task ConfigureAsync(string? instanceUrl, string? accessToken, string apiVersion);
 Task<IEnumerable<object>> CreateAccountsAsync(IEnumerable<AccountDto> accounts);
 Task<object?> GetAccountAsync(string id);
 Task<object> UpdateAccountAsync(string id, AccountDto account);
 Task DeleteAccountAsync(string id);
 }
}
