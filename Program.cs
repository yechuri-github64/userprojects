using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using acc-sf-test.Models;
using acc-sf-test.Services;
using acc-sf-test.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
 .AddEnvironmentVariables()
 .AddCommandLine(args);

// Bind settings with fallback defaults
var salesforceSection = builder.Configuration.GetSection("Salesforce");
var salesforceSettings = new SalesforceSettings();
salesforceSection.Bind(salesforceSettings);

// Apply fallback defaults
salesforceSettings.Provider ??= "Salesforce";
salesforceSettings.ApiVersion ??= "59.0";
if (string.IsNullOrWhiteSpace(salesforceSettings.TokenUrl))
{
 salesforceSettings.TokenUrl = salesforceSettings.UseSandbox ? "https://test.salesforce.com/services/oauth2/token" : "https://login.salesforce.com/services/oauth2/token";
}
if (string.IsNullOrWhiteSpace(salesforceSettings.AuthUrl))
{
 salesforceSettings.AuthUrl = salesforceSettings.UseSandbox ? "https://test.salesforce.com" : "https://login.salesforce.com";
}
if (string.IsNullOrWhiteSpace(salesforceSettings.InstanceUrl))
{
 salesforceSettings.InstanceUrl = "";
}

// Validate critical values at startup
var loggerFactory = LoggerFactory.Create(lb => lb.AddConsole());
var startupLogger = loggerFactory.CreateLogger("Startup");

bool invalid = false;
if (string.IsNullOrWhiteSpace(salesforceSettings.ClientId))
{
 startupLogger.LogWarning("Salesforce ClientId is empty. This may prevent authentication.");
}
if (string.IsNullOrWhiteSpace(salesforceSettings.ClientSecret))
{
 startupLogger.LogWarning("Salesforce ClientSecret is empty. This may prevent authentication.");
}
if (string.IsNullOrWhiteSpace(salesforceSettings.Username))
{
 startupLogger.LogWarning("Salesforce Username is empty. This may prevent authentication.");
}
if (string.IsNullOrWhiteSpace(salesforceSettings.Password))
{
 startupLogger.LogWarning("Salesforce Password is empty. This may prevent authentication.");
}
// TokenUrl and AuthUrl and InstanceUrl must be valid URIs if present
if (!string.IsNullOrWhiteSpace(salesforceSettings.TokenUrl) && !Uri.TryCreate(salesforceSettings.TokenUrl, UriKind.Absolute, out _))
{
 startupLogger.LogError("Salesforce TokenUrl is not a valid absolute URI: {TokenUrl}", salesforceSettings.TokenUrl);
 invalid = true;
}
if (!string.IsNullOrWhiteSpace(salesforceSettings.AuthUrl) && !Uri.TryCreate(salesforceSettings.AuthUrl, UriKind.Absolute, out _))
{
 startupLogger.LogError("Salesforce AuthUrl is not a valid absolute URI: {AuthUrl}", salesforceSettings.AuthUrl);
 invalid = true;
}
if (!string.IsNullOrWhiteSpace(salesforceSettings.InstanceUrl) && !Uri.TryCreate(salesforceSettings.InstanceUrl, UriKind.Absolute, out _))
{
 startupLogger.LogWarning("Salesforce InstanceUrl is not a valid absolute URI: {InstanceUrl}. It may be obtained after authentication.", salesforceSettings.InstanceUrl);
}

if (invalid)
{
 throw new ArgumentException("Invalid Salesforce configuration detected. See logs for details.");
}

builder.Services.AddSingleton(salesforceSettings);

// Configure named HttpClient for Salesforce; InstanceUrl will be set dynamically after authentication in repository/service, but register default client
builder.Services.AddHttpClient("salesforce-client");

// DI registrations
builder.Services.AddScoped<ISalesforceRepository, SalesforceRepository>();
builder.Services.AddScoped<ISalesforceService, SalesforceService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
