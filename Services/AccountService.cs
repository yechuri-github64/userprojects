using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using acc-sf-test.Models;

namespace acc-sf-test.Services
{
 public class AccountService : IAccountService
 {
 private readonly ISalesforceService _sfService;
 private readonly ILogger<AccountService> _logger;

 public AccountService(ISalesforceService sfService, ILogger<AccountService> logger)
 {
 _sfService = sfService;
 _logger = logger;
 }

 public async Task<IEnumerable<object>> CreateAccountsAsync(IEnumerable<AccountRequest> requests)
 {
 var list = requests.Select(r => new AccountDto { Name = r.Name, Phone = r.Phone, Website = r.Website }).ToList();
 _logger.LogInformation("Creating {Count} accounts in Salesforce.", list.Count);
 var results = await _sfService.CreateAccountsAsync(list);
 return results;
 }

 public async Task<object?> GetAccountAsync(string id)
 {
 _logger.LogInformation("Retrieving account {Id}", id);
 return await _sfService.GetAccountAsync(id);
 }

 public async Task<object> UpdateAccountAsync(string id, AccountRequest request)
 {
 _logger.LogInformation("Updating account {Id}", id);
 var dto = new AccountDto { Name = request.Name, Phone = request.Phone, Website = request.Website };
 return await _sfService.UpdateAccountAsync(id, dto);
 }

 public async Task DeleteAccountAsync(string id)
 {
 _logger.LogInformation("Deleting account {Id}", id);
 await _sfService.DeleteAccountAsync(id);
 }
 }
}
