using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using createweatherapp.Models;

namespace createweatherapp.Services
{
 public class WeatherService : IWeatherService
 {
 private readonly IHttpClientFactory _httpClientFactory;
 private readonly IConfiguration _configuration;
 private readonly ILogger<WeatherService> _logger;

 public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WeatherService> logger)
 {
 _httpClientFactory = httpClientFactory;
 _configuration = configuration;
 _logger = logger;
 }

 public async Task<WeatherResponse> GetWeatherAsync(string city)
 {
 if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("city is required", nameof(city));
 var apiKey = _configuration["OpenWeather:ApiKey"];
 var baseUrl = _configuration["OpenWeather:BaseUrl"];
 if (string.IsNullOrWhiteSpace(apiKey)) throw new InvalidOperationException("OpenWeather ApiKey is not configured.");
 if (string.IsNullOrWhiteSpace(baseUrl)) throw new InvalidOperationException("OpenWeather BaseUrl is not configured.");

 var url = $"{baseUrl}?q={Uri.EscapeDataString(city)}&appid={Uri.EscapeDataString(apiKey)}&units=metric";
 var client = _httpClientFactory.CreateClient();
 HttpResponseMessage resp;
 try
 {
 resp = await client.GetAsync(url);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "HTTP request to weather provider failed.");
 throw new InvalidOperationException("Failed to contact weather provider.", ex);
 }

 var content = await resp.Content.ReadAsStringAsync();
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogWarning("Weather provider returned non-success status {Status}. Content: {Content}", resp.StatusCode, content);
 throw new InvalidOperationException($"Weather provider returned status {(int)resp.StatusCode}: {resp.ReasonPhrase}");
 }

 try
 {
 using var doc = JsonDocument.Parse(content);
 var root = doc.RootElement.Clone();
 return new WeatherResponse { Data = root };
 }
 catch (JsonException ex)
 {
 _logger.LogError(ex, "Failed to parse weather provider response.");
 throw new InvalidOperationException("Invalid JSON received from weather provider.", ex);
 }
 }
 }
}
