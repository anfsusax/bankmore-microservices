using BankMore.Auth.Application.Abstractions;
using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Exceptions;
using BankMore.Auth.Domain.Repositories;
using MediatR;

namespace BankMore.Auth.Application.Commands
{
    public class CriarContaCorrenteCommandHandler : IRequestHandler<CriarContaCorrenteCommand, Guid>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IUsuarioAtual _usuarioAtual;

        public CriarContaCorrenteCommandHandler(IContaCorrenteRepository repository, IUsuarioAtual usuarioAtual)
        {
            _repository = repository;
            _usuarioAtual = usuarioAtual;
        }

        public async Task<Guid> Handle(CriarContaCorrenteCommand request, CancellationToken cancellationToken)
        {
            // O titular vem do token, nunca do corpo da requisição.
            var idUsuario = _usuarioAtual.ObterIdUsuario();

            if (await _repository.ObterPorUsuarioAsync(idUsuario) is not null)
                throw new RegraDeNegocioException("ACCOUNT_ALREADY_EXISTS", "O usuário já possui uma conta corrente.");

            if (await _repository.NumeroExisteAsync(request.Numero))
                throw new ArgumentException("Número da conta já existe.");

            var salt = Guid.NewGuid().ToString("N");
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha + salt, BCrypt.Net.BCrypt.GenerateSalt());

            var conta = ContaCorrente.Criar(idUsuario, request.Numero, request.Nome, senhaHash, salt);

            await _repository.AdicionarAsync(conta);
            return conta.Id;
        }
    }
}
