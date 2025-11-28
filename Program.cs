using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services;

var host = new HostBuilder()
 .ConfigureFunctionsWorkerDefaults()
 .ConfigureServices((context, services) =>
 {
 services.AddSingleton<IAccountService, AccountService>();
 services.AddLogging();
 })
 .Build();

await host.RunAsync();