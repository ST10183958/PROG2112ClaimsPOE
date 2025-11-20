using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PROG2112ClaimsPOE.Models;

namespace PROG2112ClaimsPOE.Data
{
    public class ClaimDbContext : IdentityDbContext<IdentityUser>
    {

        public ClaimDbContext(DbContextOptions<ClaimDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClaimModel> ClaimTable { get; set; }
        public DbSet<ApprovalLog> ApprovalLogs { get; set; }
    }
}
