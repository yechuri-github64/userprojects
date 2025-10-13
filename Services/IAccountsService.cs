using System.Collections.Generic;
using System.Threading.Tasks;
using accounts_management.Models;

namespace accounts_management.Services
{
    public interface IAccountsService
    {
        Task<IEnumerable<AccountDto>> GetAllAsync();
        Task<AccountDto?> GetByIdAsync(int id);
        Task<AccountDto> CreateAsync(AccountCreateDto dto);
        Task<IEnumerable<AccountDto>> CreateBatchAsync(IEnumerable<AccountCreateDto> dtos);
        Task<AccountDto?> UpdateAsync(int id, AccountUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
