using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Services;
using System.Threading.Tasks;

var host = new HostBuilder()
 .ConfigureFunctionsWorkerDefaults()
 .ConfigureServices(services =>
 {
 services.AddHttpClient();
 services.AddSingleton<IAccountService, AccountService>();
 })
 .Build();

await host.RunAsync();