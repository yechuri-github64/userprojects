using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Configuration;
using Services;
using RestSharp;

namespace AccountsManager
{
 public class Program
 {
 public static void Main(string[] args)
 {
 var host = new HostBuilder()
 .ConfigureFunctionsWorkerDefaults()
 .ConfigureServices((context, services) =>
 {
 var config = context.Configuration;
 var pgUrl = config["POSTGRESQL_URL"] ?? Environment.GetEnvironmentVariable("POSTGRESQL_URL");
 var apiKey = config["POSTGRESQL_API_KEY"] ?? Environment.GetEnvironmentVariable("POSTGRESQL_API_KEY");
 if (string.IsNullOrWhiteSpace(pgUrl))
 {
 throw new InvalidOperationException("POSTGRESQL_URL is not configured.");
 }
 var client = new RestClient(pgUrl);
 services.AddSingleton(client);
 services.AddSingleton<IAccountsService>(sp =>
 {
 var logger = sp.GetRequiredService<ILogger<Services.AccountsService>>();
 return new AccountsService(client, apiKey ?? string.Empty, logger);
 });
 })
 .Build();

 host.Run();
 }
 }
}
