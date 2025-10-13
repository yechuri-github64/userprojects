using accounts_management.Data;
using accounts_management.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080");

// Configure services
builder.Services.AddControllers();

// Database configuration
var dbSection = builder.Configuration.GetSection("Database");
var provider = dbSection.GetValue<string>("Provider") ?? "mysql";
var server = dbSection.GetValue<string>("Server") ?? "localhost";
var port = dbSection.GetValue<int?>("Port") ?? 3306;
var database = dbSection.GetValue<string>("Database") ?? "accountsdb";
var user = dbSection.GetValue<string>("User") ?? "root";
var password = dbSection.GetValue<string>("Password") ?? string.Empty;

var connectionString =
    $"Server={server};Port={port};Database={database};User={user};Password={password};";

if (!string.Equals(provider, "mysql", System.StringComparison.OrdinalIgnoreCase))
{
    throw new System.InvalidOperationException($"Unsupported provider: {provider}");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddScoped<IAccountsService, AccountsService>();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var logger = context
            .RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger("GlobalExceptionHandler");
        if (exceptionHandlerPathFeature?.Error != null)
        {
            logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception");
        }
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(
            new { title = "An unexpected error occurred.", status = 500 }
        );
    });
});

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
