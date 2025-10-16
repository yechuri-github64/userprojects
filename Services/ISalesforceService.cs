using System.Collections.Generic;
using System.Threading.Tasks;
using accounts_sf_sa.Models;

namespace accounts_sf_sa.Services
{
    public interface ISalesforceService
    {
        Task<List<CreateResult>> CreateMultipleAccountsAsync(
            List<Dictionary<string, object>> accounts
        );
        Task<Dictionary<string, object>?> GetAccountAsync(string id);
        Task<bool> UpdateAccountAsync(string id, Dictionary<string, object> fields);
        Task<bool> DeleteAccountAsync(string id);
    }
}
