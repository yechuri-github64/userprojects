using accounts_sf_sa.Models;
using Microsoft.EntityFrameworkCore;

namespace accounts_sf_sa.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<RequestLog> RequestLogs => Set<RequestLog>();
    }
}
