namespace BankMore.Auth.Application.Abstractions
{
    /// <summary>
    /// Escopa a chave enviada pelo cliente ao usuário autenticado.
    /// Sem isso, a chave é global e um cliente consegue "queimar" a chave de outro.
    /// </summary>
    public static class ChaveIdempotencia
    {
        public const int TamanhoMaximo = 128;

        public static string Escopar(Guid idUsuario, string chaveDoCliente)
        {
            if (string.IsNullOrWhiteSpace(chaveDoCliente))
                throw new ArgumentException("Chave de idempotência é obrigatória.");

            if (chaveDoCliente.Length > TamanhoMaximo)
                throw new ArgumentException($"Chave de idempotência deve ter no máximo {TamanhoMaximo} caracteres.");

            return $"{idUsuario:N}:{chaveDoCliente.Trim()}";
        }
    }
}
