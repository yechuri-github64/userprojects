using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Services
{
 public class ApiKeyValidator : IApiKeyValidator
 {
 private readonly string? _configuredKey;

 public ApiKeyValidator(IConfiguration configuration)
 {
 _configuredKey = configuration["ApiKey"];
 }

 public Task<bool> ValidateAsync(string? apiKey)
 {
 // If no configured API key, allow anonymous (helps local dev). If configured, require match.
 if (string.IsNullOrWhiteSpace(_configuredKey))
 return Task.FromResult(true);

 var valid = !string.IsNullOrEmpty(apiKey) && apiKey == _configuredKey;
 return Task.FromResult(valid);
 }
 }
}
