using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using acc_sf_test.Models;

namespace acc_sf_test.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account> GetByIdAsync(Guid id);
        Task<IEnumerable<Account>> CreateAsync(IEnumerable<AccountRequest> requests);
        Task<Account> UpdateAsync(Guid id, AccountRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
