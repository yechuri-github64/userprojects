using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Azure.Functions.Worker;
using SalesforceAccountFunctions.Services;
using SalesforceAccountFunctions.Helpers;

var host = new HostBuilder()
 .ConfigureFunctionsWorkerDefaults(builder =>
 {
 // builder can be used to configure middleware, etc.
 })
 .ConfigureAppConfiguration((context, config) =>
 {
 config.AddEnvironmentVariables();
 })
 .ConfigureServices((context, services) =>
 {
 var configuration = context.Configuration;

 services.AddHttpClient();
 services.AddSingleton<SalesforceService>();
 services.AddSingleton<PostgresHelper>();
 services.AddSingleton<ILoggingService, LoggingService>();

 // Configure PostgresHelper with connection string from config
 services.AddOptions<PostgresOptions>().Configure(opts =>
 {
 opts.ConnectionString = configuration["Postgres:ConnectionString"] ?? string.Empty;
 });

 // Configure Salesforce options via environment
 services.AddOptions<SalesforceOptions>().Configure(opts =>
 {
 opts.ClientId = configuration["Salesforce:ClientId"] ?? string.Empty;
 opts.ClientSecret = configuration["Salesforce:ClientSecret"] ?? string.Empty;
 opts.Username = configuration["Salesforce:Username"] ?? string.Empty;
 opts.Password = configuration["Salesforce:Password"] ?? string.Empty;
 opts.TokenUrl = configuration["Salesforce:TokenUrl"] ?? "https://login.salesforce.com/services/oauth2/token";
 opts.ApiVersion = configuration["Salesforce:ApiVersion"] ?? "v56.0";
 });
 })
 .ConfigureLogging((context, logging) =>
 {
 logging.AddConsole();
 })
 .Build();

await host.RunAsync();
