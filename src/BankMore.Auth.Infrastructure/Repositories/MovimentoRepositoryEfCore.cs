using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;
using BankMore.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Auth.Infrastructure.Repositories
{
    public class MovimentoRepositoryEfCore : IMovimentoRepository
    {
        private readonly BankMoreDbContext _context;

        public MovimentoRepositoryEfCore(BankMoreDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Movimento movimento)
        {
            await _context.Movimentos.AddAsync(movimento);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> CalcularSaldoAsync(Guid contaId)
        {
            var creditos = await _context.Movimentos
                .Where(m => m.IdContaCorrente == contaId && m.TipoMovimento == "C")
                .SumAsync(m => (decimal?)m.Valor) ?? 0m;

            var debitos = await _context.Movimentos
                .Where(m => m.IdContaCorrente == contaId && m.TipoMovimento == "D")
                .SumAsync(m => (decimal?)m.Valor) ?? 0m;

            return creditos - debitos;
        }

        public async Task<bool> ExisteIdempotenciaAsync(string chaveIdempotencia)
        {
            return await _context.Movimentos
                .AnyAsync(m => m.ChaveIdempotencia == chaveIdempotencia);
        }
    }
}
