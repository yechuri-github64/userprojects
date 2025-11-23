using System.Threading.Tasks;
using demo_test_accounts_salesforce_app.Models;

namespace demo_test_accounts_salesforce_app.Services
{
 public interface ISalesforceService
 {
 Task<Account> GetAccountAsync(string id);
 Task<Account> CreateAccountAsync(Account account);
 Task<bool> UpdateAccountAsync(string id, Account account);
 Task<bool> DeleteAccountAsync(string id);
 }
}
