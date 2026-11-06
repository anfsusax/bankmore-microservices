namespace BankMore.Auth.Domain.Repositories
{
    /// <summary>
    /// Porta para depósito (crédito) e saque (débito) atômicos.
    /// O saldo materializado (contacorrente.saldo) e o movimento (ledger) são gravados
    /// na mesma transação: é a única fonte de verdade do saldo.
    /// </summary>
    public interface IMovimentacaoFinanceira
    {
        /// <param name="tipo">"C" = crédito, "D" = débito.</param>
        /// <exception cref="Exceptions.RegraDeNegocioException">Conta inativa ou saldo insuficiente.</exception>
        /// <exception cref="Exceptions.ConflitoDeIdempotenciaException">Chave reutilizada com outro conteúdo.</exception>
        Task<ResultadoOperacaoFinanceira> MovimentarAsync(
            Guid idConta,
            string tipo,
            decimal valor,
            string chaveIdempotencia,
            string descricao,
            CancellationToken cancellationToken = default);
    }
}
