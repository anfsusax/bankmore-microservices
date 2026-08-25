using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;
using BankMore.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Auth.Infrastructure.Repositories
{
    public class IdempotenciaRepositoryEfCore : IIdempotenciaRepository
    {
        private readonly BankMoreDbContext _context;

        public IdempotenciaRepositoryEfCore(BankMoreDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteAsync(string chaveIdempotencia)
        {
            return await _context.Idempotencias
                .AnyAsync(i => i.Chave == chaveIdempotencia);
        }

        public async Task SalvarAsync(string chaveIdempotencia, string resultado)
        {
            var idempotencia = new Idempotencia(chaveIdempotencia, string.Empty, resultado);
            await _context.Idempotencias.AddAsync(idempotencia);
            await _context.SaveChangesAsync();
        }
    }
}
