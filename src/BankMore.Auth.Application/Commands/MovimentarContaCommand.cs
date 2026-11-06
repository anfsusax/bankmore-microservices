using BankMore.Auth.Domain.Repositories;
using MediatR;

namespace BankMore.Auth.Application.Commands
{
    /// <param name="Tipo">"C" (depósito/crédito) ou "D" (saque/débito).</param>
    /// <param name="NumeroConta">
    /// Opcional. Se informado e diferente da conta do titular, só é aceito para CRÉDITO
    /// (depósito identificado em conta de terceiro). Débito é sempre na conta do próprio titular.
    /// </param>
    public record MovimentarContaCommand(
         string Tipo,
         decimal Valor,
         string ChaveIdempotencia,
         int? NumeroConta = null,
         string? Descricao = null
     ) : IRequest<ResultadoOperacaoFinanceira>;
}
