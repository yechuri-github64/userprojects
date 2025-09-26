using Microsoft.EntityFrameworkCore;
using lucky_number.Models;

namespace lucky_number.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<GeneratedNumber> GeneratedNumbers { get; set; }
    }
}
