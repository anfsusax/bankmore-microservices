using BankMore.Auth.Domain.Repositories;
using MediatR;

namespace BankMore.Auth.Application.Commands
{
    /// <summary>
    /// A conta de ORIGEM não faz parte do comando: ela vem do usuário autenticado.
    /// Aceitar a origem do cliente era uma falha BOLA (OWASP API1): qualquer um movia dinheiro de qualquer conta.
    /// </summary>
    public record RealizarTransferenciaCommand(int NumeroContaDestino, decimal Valor, string ChaveIdempotencia)
        : IRequest<ResultadoOperacaoFinanceira>;
}
