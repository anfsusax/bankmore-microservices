using BankMore.Auth.Domain.Exceptions;
using BankMore.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Auth.Infrastructure.Repositories
{
    /// <summary>
    /// Quando um UPDATE condicional (WHERE ativo AND saldo >= valor) afeta 0 linhas,
    /// descobre o MOTIVO para devolver um erro de negócio claro ao cliente.
    /// Roda dentro da mesma transação, então enxerga o mesmo estado.
    /// </summary>
    internal static class DiagnosticoDeConta
    {
        public static async Task<Exception> ExplicarFalhaAsync(
            BankMoreDbContext context, Guid idConta, string papel, CancellationToken ct)
        {
            var conta = await context.ContasCorrente
                .AsNoTracking()
                .Where(c => c.Id == idConta)
                .Select(c => new { c.Ativo })
                .FirstOrDefaultAsync(ct);

            if (conta is null)
                return new RecursoNaoEncontradoException("INVALID_ACCOUNT", $"Conta de {papel} não encontrada.");

            if (!conta.Ativo)
                return new RegraDeNegocioException("INACTIVE_ACCOUNT", $"Conta de {papel} está inativa.");

            return new RegraDeNegocioException("INSUFFICIENT_FUNDS", "Saldo insuficiente.");
        }
    }
}
