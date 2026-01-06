using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using accounts_management_c_sharp.Data;
using accounts_management_c_sharp.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// load configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();

// Configure Kestrel to listen on port 8080
builder.WebHost.ConfigureKestrel(options =>
{
 options.ListenAnyIP(8080);
 options.AddServerHeader = false;
});

// Services
builder.Services.AddControllers();
builder.Services.AddSingleton<ApplicationDbContext>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("salesforce");
builder.Services.AddScoped<ISalesforceClient, SalesforceClient>();

builder.Services.AddLogging();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();
