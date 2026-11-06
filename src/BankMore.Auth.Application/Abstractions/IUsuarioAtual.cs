namespace BankMore.Auth.Application.Abstractions
{
    /// <summary>
    /// Quem está fazendo a requisição. A Application não conhece HTTP nem JWT:
    /// a implementação fica na API (UsuarioAtualHttp). Isso deixa os handlers testáveis
    /// e prontos para rodar fora de um controller (ex.: consumidor Kafka).
    /// </summary>
    public interface IUsuarioAtual
    {
        /// <exception cref="UnauthorizedAccessException">Requisição sem usuário autenticado.</exception>
        Guid ObterIdUsuario();
    }
}
