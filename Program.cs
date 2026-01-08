using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Configuration;
using RandomNumberGenerator.Services;

var host = new HostBuilder()
 .ConfigureFunctionsWorkerDefaults()
 .ConfigureAppConfiguration((context, builder) =>
 {
 builder.AddEnvironmentVariables();
 })
 .ConfigureServices((context, services) =>
 {
 services.AddSingleton<IRandomService, RandomService>();
 services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
 })
 .ConfigureLogging((context, logging) =>
 {
 logging.AddConsole();
 })
 .Build();

host.Run();
