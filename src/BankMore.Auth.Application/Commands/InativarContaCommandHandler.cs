using BankMore.Auth.Application.Abstractions;
using BankMore.Auth.Domain.Exceptions;
using BankMore.Auth.Domain.Repositories;
using MediatR;

namespace BankMore.Auth.Application.Commands
{
    public class InativarContaCommandHandler : IRequestHandler<InativarContaCommand, Unit>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IUsuarioAtual _usuarioAtual;

        public InativarContaCommandHandler(IContaCorrenteRepository repository, IUsuarioAtual usuarioAtual)
        {
            _repository = repository;
            _usuarioAtual = usuarioAtual;
        }

        public async Task<Unit> Handle(InativarContaCommand request, CancellationToken cancellationToken)
        {
            var idUsuario = _usuarioAtual.ObterIdUsuario();

            var conta = await _repository.ObterPorUsuarioAsync(idUsuario)
                ?? throw new RecursoNaoEncontradoException("INVALID_ACCOUNT", "Conta não encontrada.");

            if (!conta.Ativo)
                throw new RegraDeNegocioException("INACTIVE_ACCOUNT", "Conta já está inativa.");

            if (!BCrypt.Net.BCrypt.Verify(request.Senha + conta.Salt, conta.Senha))
                throw new UnauthorizedAccessException("Senha incorreta.");

            conta.Inativar();
            await _repository.AtualizarAsync(conta);

            return Unit.Value;
        }
    }
}
