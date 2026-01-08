using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AccountManagerFunctionApp.Repositories;
using AccountManagerFunctionApp.Helpers.Db;

var host = new HostBuilder()
 .ConfigureFunctionsWorkerDefaults()
 .ConfigureAppConfiguration((context, builder) =>
 {
 builder.AddEnvironmentVariables();
 })
 .ConfigureServices((context, services) =>
 {
 services.AddSingleton<DbHelper>();
 services.AddSingleton<IAccountRepository, AccountRepository>();
 })
 .ConfigureLogging((context, logging) =>
 {
 logging.AddConsole();
 })
 .Build();

host.Run();
