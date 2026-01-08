using System.Threading.Tasks;

namespace RandomNumberGenerator.Services
{
 public interface IApiKeyValidator
 {
 Task<bool> ValidateAsync(string? apiKey);
 }
}
