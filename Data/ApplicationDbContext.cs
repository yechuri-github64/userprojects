using System;
using Microsoft.EntityFrameworkCore;

namespace DatatableLambda.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string? conn = Environment.GetEnvironmentVariable("MYSQL_CONN");
                if (!string.IsNullOrEmpty(conn))
                {
                    optionsBuilder.UseMySql(conn, ServerVersion.AutoDetect(conn));
                }
            }
        }
    }

    public class Account
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        // Additional optional fields
        public string? AdditionalInfo { get; set; }
    }
}
