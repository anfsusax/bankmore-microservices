using BankMore.Auth.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BankMore.Auth.Application.Commands
{
    public class InativarContaCommandHandler : IRequestHandler<InativarContaCommand, Unit>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InativarContaCommandHandler(IContaCorrenteRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(InativarContaCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
 
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var contaId))
                throw new UnauthorizedAccessException();

            var conta = await _repository.ObterPorIdAsync(contaId);
            if (conta is null)
                throw new ArgumentException("Conta não encontrada", "INVALID_ACCOUNT");

            if (!conta.Ativo)
                throw new ArgumentException("Conta já está inativa", "INACTIVE_ACCOUNT");

            var hashInformado = BCrypt.Net.BCrypt.HashPassword(request.Senha + conta.Salt);
            if (!BCrypt.Net.BCrypt.Verify(request.Senha + conta.Salt, conta.Senha))
                throw new ArgumentException("Senha incorreta", "INVALID_PASSWORD");

            conta.Inativar();
            await _repository.AtualizarAsync(conta);

            return Unit.Value;
        }
    }
}
