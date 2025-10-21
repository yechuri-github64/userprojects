using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DatatableLambda.Data;
using DotNetEnv;

namespace DatatableLambda.Services
{
    public static class Service
    {
        /// <summary>
        /// Configure services, including registering ApplicationDbContext using MYSQL_CONN from .env or environment.
        /// DotNetEnv.Env.Load() is executed first to ensure .env values are available.
        /// </summary>
        public static void ConfigureServices(IServiceCollection services)
        {
            // Load environment from .env first
            DotNetEnv.Env.Load();
            string? conn = DotNetEnv.Env.GetValue("MYSQL_CONN") ?? Environment.GetEnvironmentVariable("MYSQL_CONN");

            if (string.IsNullOrEmpty(conn))
            {
                throw new InvalidOperationException("MYSQL_CONN is not configured. Ensure .env or environment variable is set.");
            }

            // Register DbContext using the connection string
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // Use the connection in the UseMySql call (ServerVersion.AutoDetect will be used to determine server version)
                options.UseMySql(conn, ServerVersion.AutoDetect(conn));
            });
        }
    }
}
