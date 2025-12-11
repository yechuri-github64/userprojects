using Microsoft.EntityFrameworkCore;
using OrdersManagement.Models;

namespace OrdersManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Account> Accounts { get; set; }
    }
}
