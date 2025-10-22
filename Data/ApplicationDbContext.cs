using Microsoft.EntityFrameworkCore;
using acc_sf_test.Models;

namespace acc_sf_test.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
    }
}
