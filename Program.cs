using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using lucky_number.Data;
using lucky_number.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services and configure in-memory EF Core provider
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("LuckyNumberDb"));

builder.Services.AddScoped<IGeneratedNumberService, GeneratedNumberService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
