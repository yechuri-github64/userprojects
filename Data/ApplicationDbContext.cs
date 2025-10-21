using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace DatatableLambda.Data
{
    /// <summary>
    /// EF Core DbContext for the datatable project. Exposes DbSet<Account> mapped to 'accounts' table.
    /// OnConfiguring reads MYSQL_CONN from .env or environment and configures MySQL.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Ensure .env is loaded first
                DotNetEnv.Env.Load();
                string? conn = DotNetEnv.Env.GetValue("MYSQL_CONN") ?? Environment.GetEnvironmentVariable("MYSQL_CONN");

                if (string.IsNullOrEmpty(conn))
                {
                    throw new InvalidOperationException("MYSQL_CONN is not configured. Ensure .env or environment variable is set.");
                }

                // Use conn in the UseMySql call
                optionsBuilder.UseMySql(conn, ServerVersion.AutoDetect(conn));
            }

            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Simple Account entity matching the 'accounts' table.
        /// Add or adjust properties to match your database schema.
        /// </summary>
        public class Account
        {
            [Key]
            public int Id { get; set; }

            public string? Username { get; set; }

            public string? Email { get; set; }

            public DateTime CreatedAt { get; set; }
        }
    }
}
