using Microsoft.EntityFrameworkCore;
using BankMore.Auth.Domain.Entities;

namespace BankMore.Auth.Infrastructure.Persistence
{
    public class BankMoreDbContext : DbContext
    {
        public BankMoreDbContext(DbContextOptions<BankMoreDbContext> options)
            : base(options) { }

        public DbSet<ContaCorrente> ContasCorrente { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new Configurations.ContaCorrenteConfiguration());
        }
    }
}
