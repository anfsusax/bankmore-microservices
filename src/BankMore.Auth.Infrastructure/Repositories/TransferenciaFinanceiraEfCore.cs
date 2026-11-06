using System.Data;
using System.Globalization;
using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Exceptions;
using BankMore.Auth.Domain.Repositories;
using BankMore.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Auth.Infrastructure.Repositories
{
    /// <summary>
    /// Transferência interna como uma única unidade atômica no banco.
    ///
    /// Decisões (ver Livro, cap. 00):
    /// - READ COMMITTED + UPDATE condicional: o próprio UPDATE trava a linha e reavalia
    ///   "saldo >= valor", então não há double-spend. SERIALIZABLE não acrescentava segurança aqui
    ///   e gerava erros 40001 (PostgreSQL) sob carga.
    /// - Execution strategy: deadlock e falha transitória refazem a transação INTEIRA, que é o único jeito seguro
    ///   de repetir uma operação financeira.
    /// - Idempotência com replay: a mesma chave + o mesmo conteúdo devolve o resultado original;
    ///   a mesma chave + outro conteúdo é conflito.
    /// </summary>
    public sealed class TransferenciaFinanceiraEfCore : ITransferenciaFinanceira
    {
        private readonly BankMoreDbContext _context;

        public TransferenciaFinanceiraEfCore(BankMoreDbContext context)
        {
            _context = context;
        }

        public async Task<ResultadoOperacaoFinanceira> ProcessarAsync(
            Guid idContaOrigem,
            Guid idContaDestino,
            decimal valor,
            string chaveIdempotencia,
            CancellationToken cancellationToken = default)
        {
            var requisicao = string.Create(CultureInfo.InvariantCulture,
                $"transferencia:{idContaOrigem}:{idContaDestino}:{valor:F2}");

            var estrategia = _context.Database.CreateExecutionStrategy();

            try
            {
                return await estrategia.ExecuteAsync(
                    ct => ExecutarAsync(idContaOrigem, idContaDestino, valor, chaveIdempotencia, requisicao, ct),
                    cancellationToken);
            }
            catch (DbUpdateException)
            {
                // Corrida entre duas requisições com a mesma chave: a primeira commitou,
                // esta violou a PK de idempotencia e teve rollback completo (nenhum saldo mudou).
                _context.ChangeTracker.Clear();
                var existente = await BuscarIdempotenciaAsync(chaveIdempotencia, cancellationToken);
                if (existente is null)
                    throw;

                return Replay(existente, requisicao);
            }
        }

        private async Task<ResultadoOperacaoFinanceira> ExecutarAsync(
            Guid idContaOrigem, Guid idContaDestino, decimal valor,
            string chaveIdempotencia, string requisicao, CancellationToken ct)
        {
            _context.ChangeTracker.Clear(); // cada tentativa da estratégia começa limpa

            await using var transacao = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

            var existente = await BuscarIdempotenciaAsync(chaveIdempotencia, ct);
            if (existente is not null)
                return Replay(existente, requisicao); // dispose sem commit = rollback

            var agora = DateTime.UtcNow;

            // Ordem de lock determinística: as duas linhas são SEMPRE travadas em ordem crescente de Id.
            // Sem isso, A→B e B→A simultâneas travam em ordem inversa e geram deadlock (40P01 / 1205).
            // Medido no lab: 40 transferências cruzadas → 24 erros 500 antes desta ordenação, 0 depois.
            if (idContaOrigem.CompareTo(idContaDestino) < 0)
            {
                await DebitarOrigemAsync(idContaOrigem, valor, agora, ct);
                await CreditarDestinoAsync(idContaDestino, valor, agora, ct);
            }
            else
            {
                // Creditar primeiro é seguro: se o débito falhar, o rollback desfaz o crédito.
                await CreditarDestinoAsync(idContaDestino, valor, agora, ct);
                await DebitarOrigemAsync(idContaOrigem, valor, agora, ct);
            }

            var idTransferencia = Guid.NewGuid();

            _context.Movimentos.Add(new Movimento(
                Guid.NewGuid(), idContaOrigem, agora, "D", valor, $"transferencia:{idTransferencia}:debito",
                $"Transferência {idTransferencia}"));
            _context.Movimentos.Add(new Movimento(
                Guid.NewGuid(), idContaDestino, agora, "C", valor, $"transferencia:{idTransferencia}:credito",
                $"Transferência {idTransferencia}"));
            _context.Transferencias.Add(new Transferencia(
                idTransferencia, idContaOrigem, idContaDestino, agora, valor));
            _context.Idempotencias.Add(new Idempotencia(
                chaveIdempotencia, requisicao, idTransferencia.ToString()));

            await _context.SaveChangesAsync(ct);
            await transacao.CommitAsync(ct);

            return new ResultadoOperacaoFinanceira(idTransferencia, Replay: false);
        }

        private async Task DebitarOrigemAsync(Guid idConta, decimal valor, DateTime agora, CancellationToken ct)
        {
            var linhas = await _context.ContasCorrente
                .Where(x => x.Id == idConta && x.Ativo && x.Saldo >= valor)
                .ExecuteUpdateAsync(set => set
                    .SetProperty(x => x.Saldo, x => x.Saldo - valor)
                    .SetProperty(x => x.AtualizadoEm, agora), ct);

            if (linhas == 0)
                throw await DiagnosticoDeConta.ExplicarFalhaAsync(_context, idConta, "origem", ct);
        }

        private async Task CreditarDestinoAsync(Guid idConta, decimal valor, DateTime agora, CancellationToken ct)
        {
            var linhas = await _context.ContasCorrente
                .Where(x => x.Id == idConta && x.Ativo)
                .ExecuteUpdateAsync(set => set
                    .SetProperty(x => x.Saldo, x => x.Saldo + valor)
                    .SetProperty(x => x.AtualizadoEm, agora), ct);

            if (linhas == 0)
                throw await DiagnosticoDeConta.ExplicarFalhaAsync(_context, idConta, "destino", ct);
        }

        private Task<Idempotencia?> BuscarIdempotenciaAsync(string chave, CancellationToken ct) =>
            _context.Idempotencias.AsNoTracking().FirstOrDefaultAsync(x => x.Chave == chave, ct);

        private static ResultadoOperacaoFinanceira Replay(Idempotencia existente, string requisicao)
        {
            if (!string.Equals(existente.Requisicao, requisicao, StringComparison.Ordinal))
                throw new ConflitoDeIdempotenciaException();

            return new ResultadoOperacaoFinanceira(Guid.Parse(existente.Resultado), Replay: true);
        }
    }
}
