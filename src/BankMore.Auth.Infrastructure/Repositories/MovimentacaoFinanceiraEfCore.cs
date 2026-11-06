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
    /// Depósito e saque atômicos. Antes: "checa saldo → insere movimento → recalcula saldo",
    /// em 3 idas ao banco sem transação. Dois saques simultâneos passavam pela checagem (double-spend).
    /// Agora: UPDATE condicional + movimento + idempotência na MESMA transação.
    /// </summary>
    public sealed class MovimentacaoFinanceiraEfCore : IMovimentacaoFinanceira
    {
        private readonly BankMoreDbContext _context;

        public MovimentacaoFinanceiraEfCore(BankMoreDbContext context)
        {
            _context = context;
        }

        public async Task<ResultadoOperacaoFinanceira> MovimentarAsync(
            Guid idConta,
            string tipo,
            decimal valor,
            string chaveIdempotencia,
            string descricao,
            CancellationToken cancellationToken = default)
        {
            if (tipo != "C" && tipo != "D")
                throw new ArgumentException("Tipo de movimentação inválido.", nameof(tipo));

            var requisicao = string.Create(CultureInfo.InvariantCulture, $"movimento:{idConta}:{tipo}:{valor:F2}");
            var estrategia = _context.Database.CreateExecutionStrategy();

            try
            {
                return await estrategia.ExecuteAsync(
                    ct => ExecutarAsync(idConta, tipo, valor, chaveIdempotencia, descricao, requisicao, ct),
                    cancellationToken);
            }
            catch (DbUpdateException)
            {
                _context.ChangeTracker.Clear();
                var existente = await BuscarIdempotenciaAsync(chaveIdempotencia, cancellationToken);
                if (existente is null)
                    throw;

                return Replay(existente, requisicao);
            }
        }

        private async Task<ResultadoOperacaoFinanceira> ExecutarAsync(
            Guid idConta, string tipo, decimal valor, string chaveIdempotencia,
            string descricao, string requisicao, CancellationToken ct)
        {
            _context.ChangeTracker.Clear();

            await using var transacao = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

            var existente = await BuscarIdempotenciaAsync(chaveIdempotencia, ct);
            if (existente is not null)
                return Replay(existente, requisicao);

            var agora = DateTime.UtcNow;

            var linhas = tipo == "C"
                ? await _context.ContasCorrente
                    .Where(x => x.Id == idConta && x.Ativo)
                    .ExecuteUpdateAsync(set => set
                        .SetProperty(x => x.Saldo, x => x.Saldo + valor)
                        .SetProperty(x => x.AtualizadoEm, agora), ct)
                : await _context.ContasCorrente
                    .Where(x => x.Id == idConta && x.Ativo && x.Saldo >= valor)
                    .ExecuteUpdateAsync(set => set
                        .SetProperty(x => x.Saldo, x => x.Saldo - valor)
                        .SetProperty(x => x.AtualizadoEm, agora), ct);

            if (linhas == 0)
                throw await DiagnosticoDeConta.ExplicarFalhaAsync(_context, idConta, "movimentação", ct);

            var movimento = tipo == "C"
                ? Movimento.CriarCredito(idConta, valor, chaveIdempotencia, descricao)
                : Movimento.CriarDebito(idConta, valor, chaveIdempotencia, descricao);

            _context.Movimentos.Add(movimento);
            _context.Idempotencias.Add(new Idempotencia(chaveIdempotencia, requisicao, movimento.Id.ToString()));

            await _context.SaveChangesAsync(ct);
            await transacao.CommitAsync(ct);

            return new ResultadoOperacaoFinanceira(movimento.Id, Replay: false);
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
