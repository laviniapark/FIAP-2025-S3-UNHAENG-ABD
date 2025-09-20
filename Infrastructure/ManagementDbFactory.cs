using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ManagementApp.Infrastructure ;

    public class ManagementDbFactory : IDesignTimeDbContextFactory<ManagementDb>
    {
        public ManagementDb CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            
            var cfg = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = cfg.GetConnectionString("DefaultConnection")
                     ?? throw new InvalidOperationException(
                         "Connection string 'DefaultConnection' ausente para design-time.");

            var options = new DbContextOptionsBuilder<ManagementDb>()
                .UseOracle(cs)
                .Options;

            return new ManagementDb(options);
        }
    }