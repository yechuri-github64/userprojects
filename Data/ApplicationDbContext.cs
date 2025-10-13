using accounts_management.Models;
using Microsoft.EntityFrameworkCore;

namespace accounts_management.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Account> Accounts => Set<Account>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("accounts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(320).IsRequired();
                entity
                    .Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(500)
                    .IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}
