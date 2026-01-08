using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Services
{
 public interface IRandomService
 {
 /// <summary>
 /// Generate a list of integers with provided constraints.
 /// Throws ArgumentException when parameters are invalid (e.g. unique requested but range insufficient).
 /// </summary>
 Task<List<int>> GenerateAsync(int count, int min, int max, bool unique = false, int? seed = null);
 }
}
