using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using createweatherapp.Services;

namespace createweatherapp.Controllers
{
 [ApiController]
 [Route("[controller]")]
 public class WeatherController : ControllerBase
 {
 private readonly IWeatherService _weatherService;
 private readonly ILogger<WeatherController> _logger;

 public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
 {
 _weatherService = weatherService;
 _logger = logger;
 }

 [HttpGet]
 public async Task<IActionResult> GetByQuery([FromQuery] string city)
 {
 if (string.IsNullOrWhiteSpace(city))
 {
 return BadRequest(new { error = "City is required as query parameter or route." });
 }
 try
 {
 var result = await _weatherService.GetWeatherAsync(city.Trim());
 return Ok(result.Data);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error fetching weather for city {City}", city);
 return StatusCode(500, new { error = "Failed to retrieve weather data.", details = ex.Message });
 }
 }

 [HttpGet("{city}")]
 public async Task<IActionResult> GetByRoute(string city)
 {
 return await GetByQuery(city);
 }
 }
}
