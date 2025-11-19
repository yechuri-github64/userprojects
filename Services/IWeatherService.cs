using System.Threading.Tasks;
using createweatherapp.Models;

namespace createweatherapp.Services
{
 public interface IWeatherService
 {
 Task<WeatherResponse> GetWeatherAsync(string city);
 }
}
