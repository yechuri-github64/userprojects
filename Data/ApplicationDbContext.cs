using Microsoft.EntityFrameworkCore;
using test_project_main1.Models;

namespace test_project_main1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Number> Numbers { get; set; }
    }
}
