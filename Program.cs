using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using test-acc-sf-app.Models;
using test-acc-sf-app.Services;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Bind Salesforce settings
builder.Services.Configure<SalesforceSettings>(builder.Configuration.GetSection("Salesforce"));

// Dependency injection
builder.Services.AddHttpClient<ISalesforceClient, SalesforceClient>((sp, client) =>
{
 var cfg = sp.GetRequiredService<IConfiguration>().GetSection("Salesforce");
 var instance = cfg.GetValue<string>("InstanceUrl");
 if (!string.IsNullOrEmpty(instance)) client.BaseAddress = new Uri(instance);
});

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISalesforceClient, SalesforceClient>();

builder.Services.AddControllers().AddJsonOptions(opts =>
{
 opts.JsonSerializerOptions.PropertyNamingPolicy = null; // preserve property names for Salesforce
});

var app = builder.Build();

// Configure Kestrel to use port from configuration (must be 8080 as per requirements)
var port = builder.Configuration.GetValue<int?>("Application:Port") ?? 8080;
app.Urls.Clear();
app.Urls.Add($"http://*:{port}");

if (app.Environment.IsDevelopment())
{
 app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

try
{
 app.Logger.LogInformation("Starting application on port {Port}", port);
 app.Run();
}
catch (Exception ex)
{
 app.Logger.LogCritical(ex, "Host terminated unexpectedly");
}
