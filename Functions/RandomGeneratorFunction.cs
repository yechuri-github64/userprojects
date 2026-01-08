using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using RandomNumberGenerator.Models;
using RandomNumberGenerator.Services;

namespace RandomNumberGenerator.Functions
{
 public class RandomGeneratorFunction
 {
 private readonly IRandomService _randomService;
 private readonly IApiKeyValidator _apiKeyValidator;
 private readonly ILogger _logger;

 public RandomGeneratorFunction(IRandomService randomService, IApiKeyValidator apiKeyValidator, ILoggerFactory loggerFactory)
 {
 _randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
 _apiKeyValidator = apiKeyValidator ?? throw new ArgumentNullException(nameof(apiKeyValidator));
 _logger = loggerFactory.CreateLogger<RandomGeneratorFunction>();
 }

 [Function("GenerateRandomNumbers")]
 public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "random")] HttpRequestData req)
 {
 try
 {
 // Authentication: simple API key via header x-api-key
 req.Headers.TryGetValues("x-api-key", out var headerValues);
 var providedKey = headerValues?.FirstOrDefault();
 if (!await _apiKeyValidator.ValidateAsync(providedKey))
 {
 _logger.LogWarning("Unauthorized request: missing or invalid API key.");
 var unauthorized = req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
 var err = new ErrorResponse { Error = new ErrorDetail { Code = "unauthorized", Message = "Missing or invalid API key.", Details = null } };
 unauthorized.Headers.Add("Content-Type", "application/json");
 unauthorized.WriteString(JsonSerializer.Serialize(err));
 return unauthorized;
 }

 // Parse query parameters
 var query = ParseQuery(req.Url.Query);

 int count = ParseInt(query, "count", 10);
 int min = ParseInt(query, "min", 0);
 int max = ParseInt(query, "max", 100);
 bool unique = ParseBool(query, "unique", false);
 int? seed = ParseNullableInt(query, "seed");

 // Generate numbers
 var numbers = await _randomService.GenerateAsync(count, min, max, unique, seed);

 var responsePayload = new RandomResponse
 {
 Numbers = numbers,
 Metadata = new Metadata
 {
 Min = min,
 Max = max,
 Unique = unique,
 Seed = seed
 }
 };

 var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
 response.Headers.Add("Content-Type", "application/json");
 var opts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
 response.WriteString(JsonSerializer.Serialize(responsePayload, opts));
 return response;
 }
 catch (ArgumentException aex)
 {
 _logger.LogWarning(aex, "Invalid request parameters.");
 var bad = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
 bad.Headers.Add("Content-Type", "application/json");
 var err = new ErrorResponse
 {
 Error = new ErrorDetail
 {
 Code = "invalid_request",
 Message = aex.Message,
 Details = null
 }
 };
 bad.WriteString(JsonSerializer.Serialize(err));
 return bad;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Unhandled error generating random numbers.");
 var res = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
 res.Headers.Add("Content-Type", "application/json");
 var err = new ErrorResponse
 {
 Error = new ErrorDetail
 {
 Code = "internal_error",
 Message = "An unexpected error occurred.",
 Details = new { exception = ex.Message }
 }
 };
 res.WriteString(JsonSerializer.Serialize(err));
 return res;
 }
 }

 private static Dictionary<string, string> ParseQuery(string rawQuery)
 {
 var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
 if (string.IsNullOrEmpty(rawQuery)) return dict;
 var q = rawQuery;
 if (q.StartsWith("?")) q = q.Substring(1);
 var parts = q.Split('&', StringSplitOptions.RemoveEmptyEntries);
 foreach (var p in parts)
 {
 var idx = p.IndexOf('=');
 if (idx < 0)
 {
 var k = Uri.UnescapeDataString(p);
 if (!dict.ContainsKey(k)) dict[k] = string.Empty;
 }
 else
 {
 var k = Uri.UnescapeDataString(p.Substring(0, idx));
 var v = Uri.UnescapeDataString(p.Substring(idx + 1));
 if (!dict.ContainsKey(k)) dict[k] = v;
 }
 }
 return dict;
 }

 private static int ParseInt(Dictionary<string, string> q, string key, int defaultValue)
 {
 if (q.TryGetValue(key, out var s) && int.TryParse(s, out var val)) return val;
 return defaultValue;
 }

 private static int? ParseNullableInt(Dictionary<string, string> q, string key)
 {
 if (q.TryGetValue(key, out var s) && int.TryParse(s, out var val)) return val;
 return null;
 }

 private static bool ParseBool(Dictionary<string, string> q, string key, bool defaultValue)
 {
 if (q.TryGetValue(key, out var s) && bool.TryParse(s, out var val)) return val;
 if (q.TryGetValue(key, out s))
 {
 // accept 0/1
 if (s == "0") return false;
 if (s == "1") return true;
 }
 return defaultValue;
 }
 }
}
