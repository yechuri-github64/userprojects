using System;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace DatatableLambda.Data
{
    public class Account
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        // Additional fields as needed
        public string? Phone { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        private readonly string? _conn;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            _conn = null;
        }

        // Additional constructor to accept connection if provided
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, string conn) : base(options)
        {
            _conn = conn;
        }

        public DbSet<Account> Accounts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                string? conn = _conn;

                if (string.IsNullOrEmpty(conn))
                {
                    // Try environment
                    Env.Load();
                    conn = Env.GetString("MYSQL_CONN") ?? Environment.GetEnvironmentVariable("MYSQL_CONN");
                }

                if (!string.IsNullOrEmpty(conn))
                {
                    optionsBuilder.UseMySql(conn, ServerVersion.AutoDetect(conn));
                }
            }
        }
    }
}
