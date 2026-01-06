using System.Net.Http;
using System.Threading.Tasks;

namespace accounts_management_c_sharp.Services
{
 public interface ISalesforceClient
 {
 Task<HttpResponseMessage> PostAsync(string relativeUrl, string contentJson);
 Task<HttpResponseMessage> GetAsync(string relativeUrl);
 Task<HttpResponseMessage> QueryAsync(string soql);
 Task EnsureAuthenticatedAsync();
 }
}
