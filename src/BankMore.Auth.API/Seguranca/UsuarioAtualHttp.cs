using System.Security.Claims;
using BankMore.Auth.Application.Abstractions;

namespace BankMore.Auth.API.Seguranca
{
    /// <summary>Lê o usuário autenticado do JWT. Única classe que conhece HttpContext.</summary>
    public sealed class UsuarioAtualHttp : IUsuarioAtual
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioAtualHttp(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid ObterIdUsuario()
        {
            var usuario = _httpContextAccessor.HttpContext?.User;

            var valor = usuario?.FindFirst("id")?.Value
                        ?? usuario?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? usuario?.FindFirst("sub")?.Value;

            if (!Guid.TryParse(valor, out var idUsuario) || idUsuario == Guid.Empty)
                throw new UnauthorizedAccessException("Usuário não autenticado ou token inválido.");

            return idUsuario;
        }
    }
}
