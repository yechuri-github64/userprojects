using Microsoft.EntityFrameworkCore;
using accounts_operation.Models;

namespace accounts_operation.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("accounts");
            base.OnModelCreating(modelBuilder);
        }
    }
}
