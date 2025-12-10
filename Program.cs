using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using account-mangement.Services;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Ensure app uses port 8080
builder.Configuration.AddInMemoryCollection(new[] { new KeyValuePair<string,string>("Kestrel:Endpoints:Http:Url","http://0.0.0.0:8080") });

// Configuration and logging
builder.Services.AddLogging();
builder.Services.AddControllers();

// Register ApplicationDbContext configured from appsettings
builder.Services.AddSingleton<ApplicationDbContext>();

// Register repository and service
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
