using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BankMore.Auth.Infrastructure.Persistence
{
    public class BankMoreDbContextFactory : IDesignTimeDbContextFactory<BankMoreDbContext>
    {
        public BankMoreDbContext CreateDbContext(string[] args)
        {
             
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BankMore.Auth.API"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Por padrão, usa SQL Server para migrations (ambiente local)
            var connectionString = configuration.GetConnectionString("SqlServer");

            var optionsBuilder = new DbContextOptionsBuilder<BankMoreDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new BankMoreDbContext(optionsBuilder.Options);
        }
    }
}

