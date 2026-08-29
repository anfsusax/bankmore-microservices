using System.Data;
using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;
using BankMore.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Auth.Infrastructure.Repositories
{
    /// <summary>
    /// Executa uma transferência como uma única unidade atômica no banco.
    /// </summary>
    public sealed class TransferenciaFinanceiraEfCore : ITransferenciaFinanceira
    {
        private readonly BankMoreDbContext _context;

        public TransferenciaFinanceiraEfCore(BankMoreDbContext context)
        {
            _context = context;
        }

        public async Task<Guid?> ProcessarAsync(
            Guid idContaOrigem,
            Guid idContaDestino,
            decimal valor,
            string chaveIdempotencia,
            CancellationToken cancellationToken = default)
        {
            await using var transacao = await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

            try
            {
                var jaProcessada = await _context.Idempotencias
                    .AsNoTracking()
                    .AnyAsync(x => x.Chave == chaveIdempotencia, cancellationToken);

                if (jaProcessada)
                {
                    await transacao.RollbackAsync(cancellationToken);
                    return null;
                }

                var agora = DateTime.UtcNow;

                var origemAtualizada = await _context.ContasCorrente
                    .Where(x => x.Id == idContaOrigem && x.Ativo && x.Saldo >= valor)
                    .ExecuteUpdateAsync(atualizacoes => atualizacoes
                        .SetProperty(x => x.Saldo, x => x.Saldo - valor)
                        .SetProperty(x => x.AtualizadoEm, agora), cancellationToken);

                if (origemAtualizada == 0)
                    throw new InvalidOperationException("Conta de origem inexistente, inativa ou sem saldo suficiente.");

                var destinoAtualizado = await _context.ContasCorrente
                    .Where(x => x.Id == idContaDestino && x.Ativo)
                    .ExecuteUpdateAsync(atualizacoes => atualizacoes
                        .SetProperty(x => x.Saldo, x => x.Saldo + valor)
                        .SetProperty(x => x.AtualizadoEm, agora), cancellationToken);

                if (destinoAtualizado == 0)
                    throw new InvalidOperationException("Conta de destino inexistente ou inativa.");

                var idTransferencia = Guid.NewGuid();
                var chaveDebito = $"transferencia:{idTransferencia}:debito";
                var chaveCredito = $"transferencia:{idTransferencia}:credito";

                _context.Movimentos.Add(new Movimento(
                    Guid.NewGuid(), idContaOrigem, agora, "D", valor, chaveDebito,
                    $"Transferência {idTransferencia}"));
                _context.Movimentos.Add(new Movimento(
                    Guid.NewGuid(), idContaDestino, agora, "C", valor, chaveCredito,
                    $"Transferência {idTransferencia}"));
                _context.Transferencias.Add(new Transferencia(
                    idTransferencia, idContaOrigem, idContaDestino, agora, valor));
                _context.Idempotencias.Add(new Idempotencia(
                    chaveIdempotencia,
                    $"transferencia:{idContaOrigem}:{idContaDestino}:{valor}",
                    idTransferencia.ToString()));

                await _context.SaveChangesAsync(cancellationToken);
                await transacao.CommitAsync(cancellationToken);

                return idTransferencia;
            }
            catch (DbUpdateException)
            {
                await transacao.RollbackAsync(cancellationToken);
                _context.ChangeTracker.Clear();

                var jaProcessada = await _context.Idempotencias
                    .AsNoTracking()
                    .AnyAsync(x => x.Chave == chaveIdempotencia, cancellationToken);

                if (jaProcessada)
                    return null;

                throw;
            }
            catch
            {
                await transacao.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
