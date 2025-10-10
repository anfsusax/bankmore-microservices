using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;
using BankMore.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Auth.Infrastructure.Repositories
{
    public class ContaCorrenteRepositoryEfCore : IContaCorrenteRepository
    {
        private readonly BankMoreDbContext _context;

        public ContaCorrenteRepositoryEfCore(BankMoreDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(ContaCorrente conta)
        {
            await _context.ContasCorrente.AddAsync(conta);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> NumeroExisteAsync(int numero)
        {
            return await _context.ContasCorrente
                .AnyAsync(c => c.Numero == numero);
        }

        public async Task<ContaCorrente?> ObterPorIdAsync(Guid id)
        {
            return await _context.ContasCorrente
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ContaCorrente?> ObterPorNumeroAsync(int numero)
        {
            return await _context.ContasCorrente
                .FirstOrDefaultAsync(c => c.Numero == numero);
        }

        public async Task AtualizarSaldoAsync(Guid idConta, decimal novoSaldo)
        {
            var conta = await ObterPorIdAsync(idConta);
            if (conta == null)
                throw new InvalidOperationException("Conta não encontrada");

            // Usar reflection para atualizar o saldo (propriedade private set)
            var saldoProperty = typeof(ContaCorrente).GetProperty(nameof(ContaCorrente.Saldo));
            saldoProperty?.SetValue(conta, novoSaldo);

            var atualizadoEmProperty = typeof(ContaCorrente).GetProperty(nameof(ContaCorrente.AtualizadoEm));
            atualizadoEmProperty?.SetValue(conta, DateTime.UtcNow);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ContaEstaAtivaAsync(Guid idConta)
        {
            var conta = await ObterPorIdAsync(idConta);
            return conta?.Ativo ?? false;
        }

        public async Task<decimal> ObterSaldoAsync(Guid idConta)
        {
            var conta = await ObterPorIdAsync(idConta);
            return conta?.Saldo ?? 0;
        }

        public async Task AtualizarAsync(ContaCorrente conta)
        {
            _context.ContasCorrente.Update(conta);
            await _context.SaveChangesAsync();
        }

        public async Task<ContaCorrente?> ObterPorDocumentoOuNumeroAsync(string documentoOuNumero)
        {
            if (int.TryParse(documentoOuNumero, out int numero))
            {
                return await ObterPorNumeroAsync(numero);
            }
            
            return null;
        }
    }
}

