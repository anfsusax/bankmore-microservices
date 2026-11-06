namespace BankMore.Auth.Domain.Exceptions
{
    /// <summary>
    /// Exceções com significado de negócio. A API traduz cada uma para um status HTTP
    /// (ver GlobalExceptionHandler), e a camada de domínio não conhece HTTP.
    /// </summary>
    public abstract class ExcecaoDeDominio : Exception
    {
        public string Codigo { get; }

        protected ExcecaoDeDominio(string codigo, string mensagem) : base(mensagem)
        {
            Codigo = codigo;
        }
    }

    /// <summary>O recurso existe, mas não pertence a quem está pedindo (HTTP 403).</summary>
    public sealed class AcessoNegadoException : ExcecaoDeDominio
    {
        public AcessoNegadoException(string mensagem) : base("ACCESS_DENIED", mensagem) { }
    }

    /// <summary>Conta, usuário ou transferência não encontrados (HTTP 404).</summary>
    public sealed class RecursoNaoEncontradoException : ExcecaoDeDominio
    {
        public RecursoNaoEncontradoException(string codigo, string mensagem) : base(codigo, mensagem) { }
    }

    /// <summary>Regra de negócio violada: saldo insuficiente, conta inativa... (HTTP 422).</summary>
    public sealed class RegraDeNegocioException : ExcecaoDeDominio
    {
        public RegraDeNegocioException(string codigo, string mensagem) : base(codigo, mensagem) { }
    }

    /// <summary>
    /// A mesma chave de idempotência foi reutilizada com OUTRO conteúdo (HTTP 422).
    /// Reenvio idêntico não é erro: devolve o resultado original.
    /// </summary>
    public sealed class ConflitoDeIdempotenciaException : ExcecaoDeDominio
    {
        public ConflitoDeIdempotenciaException()
            : base("IDEMPOTENCY_CONFLICT", "A chave de idempotência já foi usada com uma operação diferente.") { }
    }
}
