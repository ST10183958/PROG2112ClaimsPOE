using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PROG2112ClaimsPOE.Data
{
    public class ClaimDbContextFactory : IDesignTimeDbContextFactory<ClaimDbContext>
    {
        public ClaimDbContext CreateDbContext(string[] args)
        {
            // Read appsettings.json manually
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ClaimDbContext>();
            var connectionString = config.GetConnectionString("ClaimDb");

            optionsBuilder.UseSqlServer(connectionString);

            return new ClaimDbContext(optionsBuilder.Options);
        }
    }
}
