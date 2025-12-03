using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services;

var host = new HostBuilder()
 .ConfigureAppConfiguration((context, config) =>
 {
 config.AddEnvironmentVariables();
 })
 .ConfigureFunctionsWorkerDefaults()
 .ConfigureServices((context, services) =>
 {
 services.AddSingleton<IAccountService, AccountService>();
 })
 .ConfigureLogging((context, b) =>
 {
 b.AddConsole();
 })
 .Build();

await host.RunAsync();