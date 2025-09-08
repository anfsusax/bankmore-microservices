using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;
using MediatR;

namespace BankMore.Auth.Application.Commands
{
    public class CriarContaCorrenteCommandHandler : IRequestHandler<CriarContaCorrenteCommand, Guid>
    {
        private readonly IContaCorrenteRepository _repository;

        public CriarContaCorrenteCommandHandler(IContaCorrenteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CriarContaCorrenteCommand request, CancellationToken cancellationToken)
        {
            if (await _repository.NumeroExisteAsync(request.Numero))
                throw new ArgumentException("Número da conta já existe.");

            var salt = Guid.NewGuid().ToString("N");
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha + salt, BCrypt.Net.BCrypt.GenerateSalt());

            var conta = ContaCorrente.Criar(
                request.Numero,
                request.Nome,
                senhaHash,
                salt
            );

            await _repository.AdicionarAsync(conta);
            return conta.Id;
        }
    }
}
