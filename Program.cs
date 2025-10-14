using accounts_management.Data;
using accounts_management.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080");

builder.Services.AddControllers();

var provider = builder.Configuration.GetValue<string>("Database:Provider");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
}

if (
    !string.IsNullOrWhiteSpace(provider)
    && (
        provider.Equals("MySql", StringComparison.OrdinalIgnoreCase)
        || provider.Equals("mysql", StringComparison.OrdinalIgnoreCase)
    )
)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    );
}
else
{
    throw new InvalidOperationException(
        $"Unsupported or missing database provider: '{provider}'. Expected 'MySql'."
    );
}

builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
