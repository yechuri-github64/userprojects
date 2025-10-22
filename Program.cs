using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using acc_sf_test.Data;
using acc_sf_test.Services;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddEnvironmentVariables();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configure Kestrel to use port from configuration (default 8080)
var appPort = builder.Configuration.GetValue<int?>("Application:Port") ?? 8080;
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(appPort);
});

// Configure DbContext based on provider in configuration
var dbProvider = builder.Configuration.GetValue<string>("Database:Provider") ?? "InMemory";
if (dbProvider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("AccountsDb"));
}
else if (dbProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(conn));
}
else
{
    // Default to InMemory if unrecognized
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("AccountsDb"));
}

// Configure Salesforce HttpClient
var sfHost = builder.Configuration.GetValue<string>("Salesforce:Host");
var sfPort = builder.Configuration.GetValue<int?>("Salesforce:Port");
var sfScheme = builder.Configuration.GetValue<string>("Salesforce:Scheme") ?? "https";
var baseAddress = string.IsNullOrWhiteSpace(sfHost) ? null : new Uri($"{sfScheme}://{sfHost}{(sfPort.HasValue ? ":" + sfPort.Value : string.Empty)}");

builder.Services.AddHttpClient<SalesforceClient>(client =>
{
    if (baseAddress != null) client.BaseAddress = baseAddress;
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    // If an auth token is present in config, set Authorization header.
    var token = builder.Configuration.GetValue<string>("Salesforce:AuthToken");
    if (!string.IsNullOrWhiteSpace(token))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = true });

// DI registrations
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<SalesforceClient>();

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapControllers();

app.Run();
