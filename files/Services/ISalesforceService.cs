using acc_sf_test.Models;

namespace acc_sf_test.Services
{
 public interface ISalesforceService
 {
 Task AuthenticateAsync(CancellationToken ct = default);
 Task<HttpResponseMessage> SendApiAsync(HttpMethod method, string path, HttpContent? content = null, CancellationToken ct = default);
 }
}