namespace BankMore.Auth.Domain.Repositories
{
    /// <summary>
    /// Porta para transferências que precisam manter saldo, movimentos e idempotência consistentes.
    /// Retorna <c>null</c> quando a mesma chave de idempotência já foi processada.
    /// </summary>
    public interface ITransferenciaFinanceira
    {
        Task<Guid?> ProcessarAsync(
            Guid idContaOrigem,
            Guid idContaDestino,
            decimal valor,
            string chaveIdempotencia,
            CancellationToken cancellationToken = default);
    }
}
