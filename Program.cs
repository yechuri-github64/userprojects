using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using createweatherapp.Services;

var builder = WebApplication.CreateBuilder(args);
// Load configuration from appsettings.json (required for backend settings)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Read backend port from configuration and configure URLs (default 8080)
var backendPort = builder.Configuration.GetValue<int?>("BackendSystem:Port") ?? 8080;
builder.WebHost.UseUrls($"http://*:{backendPort}");

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddLogging();

var app = builder.Build();
app.MapControllers();
app.Run();
