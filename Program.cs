using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Services;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
 .ConfigureAppConfiguration((context, builder) =>
 {
 builder.AddEnvironmentVariables();
 })
 .ConfigureFunctionsWorkerDefaults()
 .ConfigureServices((context, services) =>
 {
 services.AddSingleton<IAccountService, AccountService>();
 services.AddLogging();
 })
 .Build();

await host.RunAsync();