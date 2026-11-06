namespace BankMore.Auth.API.Requests
{
    /// <summary>
    /// Não existe IdContaOrigem: a origem é sempre a conta do usuário autenticado.
    /// A chave de idempotência deve vir preferencialmente no header "Idempotency-Key";
    /// o campo no corpo é aceito só por compatibilidade.
    /// </summary>
    public class RealizarTransferenciaRequest
    {
        public int NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
        public string? ChaveIdempotencia { get; set; }
    }
}
