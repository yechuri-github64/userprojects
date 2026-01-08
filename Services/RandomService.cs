using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Services
{
 public class RandomService : IRandomService
 {
 public Task<List<int>> GenerateAsync(int count, int min, int max, bool unique = false, int? seed = null)
 {
 if (count <= 0)
 throw new ArgumentException("Count must be greater than 0.", nameof(count));
 if (min > max)
 throw new ArgumentException("Min must be less than or equal to Max.", nameof(min));

 long rangeSize = (long)max - (long)min + 1L;
 if (unique && rangeSize < count)
 throw new ArgumentException("Cannot generate the requested number of unique values within the specified range.");

 var rng = seed.HasValue ? new Random(seed.Value) : new Random();
 var result = new List<int>(count);

 if (unique)
 {
 // Reservoir-like selection: since range may be large, but we can use HashSet to accumulate unique values.
 var picked = new HashSet<int>();
 while (picked.Count < count)
 {
 int v = rng.Next(min, max + 1);
 picked.Add(v);
 }
 result.AddRange(picked);
 }
 else
 {
 for (int i = 0; i < count; i++)
 {
 result.Add(rng.Next(min, max + 1));
 }
 }

 return Task.FromResult(result);
 }
 }
}
