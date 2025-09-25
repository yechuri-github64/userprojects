using System.Collections.Generic;
using System.Threading.Tasks;
using test_project_main1.Models;

namespace test_project_main1.Services
{
    public interface INumberService
    {
        Task<IEnumerable<Number>> GetAllAsync();
        Task<Number?> GetByIdAsync(int id);
        Task<Number> CreateAsync(Number number);
        Task<bool> UpdateAsync(Number number);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Number>> GetDivisibleBy5AndLessThan500Async();
    }
}
