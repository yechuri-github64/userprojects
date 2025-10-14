using System;
using accounts_management.Data;
using accounts_management.Services; // ensure yrr IAccountService ka namespace yaha hai
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql;

var builder = WebApplication.CreateBuilder(args);

// configuration & logging
builder
    .Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// add controllers
builder.Services.AddControllers();

// 👇 yrr DI for IAccountService
builder.Services.AddScoped<IAccountService, AccountService>();

// database setup
var provider = builder.Configuration.GetValue<string>("Database:Provider") ?? "mysql";
if (!string.Equals(provider, "mysql", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        "Unsupported database provider. Only 'mysql' is supported."
    );
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    var host = builder.Configuration["Database:Server"] ?? "localhost";
    var port = builder.Configuration["Database:Port"] ?? "3306";
    var db = builder.Configuration["Database:Database"] ?? "accountsdb";
    var user = builder.Configuration["Database:User"] ?? "root";
    var password = builder.Configuration["Database:Password"] ?? "";
    connectionString =
        $"Server={host};Port={port};Database={db};User={user};Password={password};TreatTinyAsBoolean=true;SslMode=None";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    try
    {
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        options.UseMySql(connectionString, serverVersion);
    }
    catch
    {
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));
    }
});

// Kestrel URL
var urls = builder.Configuration["Kestrel:Endpoints:Http:Url"] ?? "http://0.0.0.0:8080";
builder.WebHost.UseUrls(urls);

var app = builder.Build();

// exception handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(appError =>
    {
        appError.Run(async context =>
        {
            var logger = context
                .RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("GlobalException");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = "An unexpected error occurred",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred on the server.",
            };
            logger.LogError("Unhandled exception occurred");
            await context.Response.WriteAsJsonAsync(problem);
        });
    });
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
