using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using account_c_sharp.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration and Logging
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register ApplicationDbContext and services
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

// Configure Kestrel to use port from configuration
var port = builder.Configuration.GetValue<int>("AppSettings:Port");
if (port <= 0) port = 8080;
app.Logger.LogInformation("Starting application on port {Port}", port);

app.Urls.Clear();
app.Urls.Add($"http://*:{port}");

app.MapControllers();

app.Run();
