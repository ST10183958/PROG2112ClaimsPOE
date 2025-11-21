using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PROG2112ClaimsPOE.Models;

namespace PROG2112ClaimsPOE.Data
{
    public class ClaimDbContext : IdentityDbContext<IdentityUser>
    {

        public DbSet<ClaimModel> ClaimTable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure the Statues column is treated as an integer
            modelBuilder.Entity<ClaimModel>()
                .Property(c => c.Statues)
                .HasConversion<int>();  // Converts Enum to Int in the DB
        }

        public DbSet<ApprovalLog> ApprovalLogs { get; set; }  // Add this line

        public ClaimDbContext(DbContextOptions<ClaimDbContext> options) : base(options)
        {
        }
    }
}
