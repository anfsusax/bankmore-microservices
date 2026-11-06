using BankMore.Auth.Application.Abstractions;
using BankMore.Auth.Domain.Exceptions;
using BankMore.Auth.Domain.Repositories;
using MediatR;

namespace BankMore.Auth.Application.Queries
{
    /// <summary>Saldo da conta do usuário autenticado. Não existe mais consulta de saldo de terceiros.</summary>
    public class GetSaldoQueryHandler : IRequestHandler<GetSaldoQuery, SaldoResult>
    {
        private readonly IUsuarioAtual _usuarioAtual;
        private readonly IContaCorrenteRepository _contas;

        public GetSaldoQueryHandler(IUsuarioAtual usuarioAtual, IContaCorrenteRepository contas)
        {
            _usuarioAtual = usuarioAtual;
            _contas = contas;
        }

        public async Task<SaldoResult> Handle(GetSaldoQuery request, CancellationToken cancellationToken)
        {
            var idUsuario = _usuarioAtual.ObterIdUsuario();

            var conta = await _contas.ObterPorUsuarioAsync(idUsuario)
                ?? throw new RecursoNaoEncontradoException("INVALID_ACCOUNT", "O usuário autenticado não possui conta corrente.");

            // Fonte única de verdade: o saldo materializado, atualizado na mesma transação do movimento.
            // Antes, cada endpoint lia de um lugar (coluna x SUM dos movimentos) e os valores divergiam.
            return new SaldoResult
            {
                IdConta = conta.Id,
                Numero = conta.Numero,
                Nome = conta.Nome,
                Ativo = conta.Ativo,
                DataHora = DateTime.UtcNow,
                Saldo = conta.Saldo
            };
        }
    }
}
