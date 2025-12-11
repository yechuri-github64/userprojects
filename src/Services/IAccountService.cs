using OrdersManagement.Models;

namespace OrdersManagement.Services
{
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(Account account);
        Task<Account> GetAccountByIdAsync(int id);
    }
}
