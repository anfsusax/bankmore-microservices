using Microsoft.EntityFrameworkCore;
using BankMore.Auth.Domain.Entities;

namespace BankMore.Auth.Infrastructure.Persistence
{
    public class BankMoreDbContext : DbContext
    {
        public BankMoreDbContext(DbContextOptions<BankMoreDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ContaCorrente> ContasCorrente { get; set; }
        public DbSet<Movimento> Movimentos { get; set; }
        public DbSet<Transferencia> Transferencias { get; set; }
        public DbSet<Idempotencia> Idempotencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankMoreDbContext).Assembly);
        }
    }
}
