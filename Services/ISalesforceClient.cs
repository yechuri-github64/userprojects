using System.Collections.Generic;
using System.Threading.Tasks;

namespace test-acc-sf-app.Services
{
 public interface ISalesforceClient
 {
 Task<string> GetAsync(string path);
 Task<object> CreateAsync(string path, object payload);
 Task<bool> PatchAsync(string path, object payload);
 Task<bool> DeleteAsync(string path);
 }
}
