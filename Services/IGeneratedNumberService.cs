using System.Collections.Generic;
using System.Threading.Tasks;
using lucky_number.Models;

namespace lucky_number.Services
{
    public interface IGeneratedNumberService
    {
        Task<IEnumerable<GeneratedNumber>> GetAllAsync();
        Task<GeneratedNumber?> GetAsync(int id);
        Task<GeneratedNumber> CreateAsync(GeneratedNumber item);
        Task UpdateAsync(GeneratedNumber item);
        Task DeleteAsync(int id);
    }
}
