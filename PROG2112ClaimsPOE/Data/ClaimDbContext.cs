using Microsoft.EntityFrameworkCore;
using PROG2112ClaimsPOE.Models;

namespace PROG2112ClaimsPOE.Data
{
    public class ClaimDbContext : DbContext
    {
        public ClaimDbContext(DbContextOptions<ClaimDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClaimModel> ClaimTable { get; set; }
    }
}
