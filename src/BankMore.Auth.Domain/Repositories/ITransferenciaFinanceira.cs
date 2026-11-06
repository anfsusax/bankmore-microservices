namespace BankMore.Auth.Domain.Repositories
{
    /// <summary>
    /// Porta para transferências INTERNAS (entre contas do próprio BankMore).
    /// Saldo, movimentos, registro da transferência e idempotência mudam numa única transação ACID.
    /// É por isso que esse fluxo fica no núcleo modular e não vira saga (ver ADR-001).
    /// </summary>
    public interface ITransferenciaFinanceira
    {
        /// <exception cref="Exceptions.ConflitoDeIdempotenciaException">Chave reutilizada com outro conteúdo.</exception>
        Task<ResultadoOperacaoFinanceira> ProcessarAsync(
            Guid idContaOrigem,
            Guid idContaDestino,
            decimal valor,
            string chaveIdempotencia,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// <paramref name="Replay"/> = true quando a chave já tinha sido processada com o MESMO conteúdo:
    /// o cliente recebe o resultado original e nenhum dinheiro se move de novo.
    /// </summary>
    public sealed record ResultadoOperacaoFinanceira(Guid Id, bool Replay);
}
