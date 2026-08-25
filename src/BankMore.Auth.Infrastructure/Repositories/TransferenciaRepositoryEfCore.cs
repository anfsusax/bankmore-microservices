using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;
using BankMore.Auth.Infrastructure.Persistence;

namespace BankMore.Auth.Infrastructure.Repositories
{
    public class TransferenciaRepositoryEfCore : ITransferenciaRepository
    {
        private readonly BankMoreDbContext _context;

        public TransferenciaRepositoryEfCore(BankMoreDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Transferencia transferencia)
        {
            await _context.Transferencias.AddAsync(transferencia);
            await _context.SaveChangesAsync();
        }
    }
}
