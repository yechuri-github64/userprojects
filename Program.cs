using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using demo_test_accounts_salesforce_app.Models;
using demo_test_accounts_salesforce_app.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind Salesforce settings from appsettings
builder.Services.Configure<SalesforceSettings>(builder.Configuration.GetSection("SalesforceSettings"));

// Add HttpClient factory
builder.Services.AddHttpClient();

// Dependency injection
builder.Services.AddSingleton<ISalesforceService, SalesforceService>();

builder.Services.AddControllers();
builder.Services.AddLogging();

var app = builder.Build();

app.MapControllers();

// Ensure application listens on configured port (default 8080)
//var port = builder.Configuration.GetSection("SalesforceSettings").GetValue<int?>("Port") ?? 8080;
//app.Urls.Clear();
//app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
