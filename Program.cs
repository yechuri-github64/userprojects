using accounts_sf_sa.Data;
using accounts_sf_sa.Models;
using accounts_sf_sa.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder
    .Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<SalesforceOptions>(builder.Configuration.GetSection("Salesforce"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("accounts-sf-sa-db");
});

builder.Services.AddHttpClient("Salesforce");

builder.Services.AddScoped<ISalesforceService, SalesforceService>();

builder
    .Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();

/*
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});
*/
var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
