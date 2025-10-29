using acc_sf_test.Models;

namespace acc_sf_test.Services
{
 public interface IAccountService
 {
 Task<IEnumerable<object>> CreateAccountsAsync(IEnumerable<AccountDto> accounts, CancellationToken ct = default);
 Task<object?> GetAccountAsync(string id, CancellationToken ct = default);
 Task<object?> UpdateAccountAsync(string id, AccountDto account, CancellationToken ct = default);
 Task<bool> DeleteAccountAsync(string id, CancellationToken ct = default);
 }
}