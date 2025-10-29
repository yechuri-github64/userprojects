using System.Text.Json;
using acc_sf_test.Models;
using acc_sf_test.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Load configuration and set urls fallback
var appUrls = builder.Configuration.GetValue<string>("Application:Urls");
if (string.IsNullOrWhiteSpace(appUrls))
{
 appUrls = "http://0.0.0.0:8080"; // fallback
}
builder.WebHost.UseUrls(appUrls);

// Configure options
builder.Services.Configure<SalesforceOptions>(builder.Configuration.GetSection("Salesforce"));
builder.Services.AddSingleton<IValidateOptions<SalesforceOptions>, SalesforceOptionsValidator>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<ISalesforceService, SalesforceService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddControllers().AddJsonOptions(opts => {
 opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

// Ensure configuration validated at startup
using (var scope = app.Services.CreateScope())
{
 var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
 var optionsMonitor = scope.ServiceProvider.GetRequiredService<IOptions<SalesforceOptions>>();
 var options = optionsMonitor.Value;

 // Validate and apply fallbacks
 try
 {
 var validator = new SalesforceOptionsValidator();
 var result = validator.Validate(options);
 if (!result.IsValid)
 {
 foreach (var err in result.Errors)
 {
 logger.LogError("Salesforce configuration validation error: {Error}", err);
 }
 throw new ApplicationException("Invalid Salesforce configuration. See logs for details.");
 }

 // Ensure token URL and auth url have safe defaults when missing
 if (string.IsNullOrWhiteSpace(options.TokenUrl))
 {
 options.TokenUrl = options.UseSandbox ? "https://test.salesforce.com/services/oauth2/token" : "https://login.salesforce.com/services/oauth2/token";
 logger.LogWarning("Salesforce TokenUrl missing, using fallback: {TokenUrl}", options.TokenUrl);
 }
 if (string.IsNullOrWhiteSpace(options.AuthUrl))
 {
 options.AuthUrl = options.UseSandbox ? "https://test.salesforce.com" : "https://login.salesforce.com";
 logger.LogWarning("Salesforce AuthUrl missing, using fallback: {AuthUrl}", options.AuthUrl);
 }
 if (string.IsNullOrWhiteSpace(options.InstanceUrl))
 {
 options.InstanceUrl = options.UseSandbox ? "https://your-sandbox.my.salesforce.com" : "https://your-instance.my.salesforce.com";
 logger.LogWarning("Salesforce InstanceUrl missing, using fallback: {InstanceUrl}", options.InstanceUrl);
 }

 // Validate URIs
 if (!Uri.TryCreate(options.TokenUrl, UriKind.Absolute, out _))
 {
 logger.LogError("Invalid TokenUrl configuration: {TokenUrl}", options.TokenUrl);
 throw new ApplicationException("Invalid TokenUrl configuration");
 }
 if (!Uri.TryCreate(options.AuthUrl, UriKind.Absolute, out _))
 {
 logger.LogError("Invalid AuthUrl configuration: {AuthUrl}", options.AuthUrl);
 throw new ApplicationException("Invalid AuthUrl configuration");
 }
 if (!Uri.TryCreate(options.InstanceUrl, UriKind.Absolute, out var instUri))
 {
 logger.LogWarning("InstanceUrl invalid or not reachable, continuing with fallback host: {InstanceUrl}", options.InstanceUrl);
 }

 logger.LogInformation("Salesforce configuration validated successfully. Provider={Provider} ApiVersion={ApiVersion} UseSandbox={UseSandbox}", options.Provider, options.ApiVersion, options.UseSandbox);
 }
 catch (Exception ex)
 {
 logger.LogError(ex, "Startup validation failed for Salesforce configuration");
 throw;
 }
}

app.MapControllers();

app.Run();