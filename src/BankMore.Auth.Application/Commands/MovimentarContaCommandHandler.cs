using BankMore.Auth.Application.Abstractions;
using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Exceptions;
using BankMore.Auth.Domain.Repositories;
using MediatR;

namespace BankMore.Auth.Application.Commands
{
    public class MovimentarContaCommandHandler : IRequestHandler<MovimentarContaCommand, ResultadoOperacaoFinanceira>
    {
        private readonly IContaCorrenteRepository _contas;
        private readonly IMovimentacaoFinanceira _movimentacao;
        private readonly IUsuarioAtual _usuarioAtual;

        public MovimentarContaCommandHandler(
            IContaCorrenteRepository contas,
            IMovimentacaoFinanceira movimentacao,
            IUsuarioAtual usuarioAtual)
        {
            _contas = contas;
            _movimentacao = movimentacao;
            _usuarioAtual = usuarioAtual;
        }

        public async Task<ResultadoOperacaoFinanceira> Handle(MovimentarContaCommand request, CancellationToken cancellationToken)
        {
            if (request.Tipo != "C" && request.Tipo != "D")
                throw new ArgumentException("Tipo de movimentação inválido. Utilize 'C' (Crédito) ou 'D' (Débito).");

            if (request.Valor <= 0)
                throw new ArgumentException("O valor da operação deve ser maior que zero.");

            var idUsuario = _usuarioAtual.ObterIdUsuario();
            var chave = ChaveIdempotencia.Escopar(idUsuario, request.ChaveIdempotencia);

            var contaDoTitular = await _contas.ObterPorUsuarioAsync(idUsuario)
                ?? throw new RecursoNaoEncontradoException("INVALID_ACCOUNT", "O usuário autenticado não possui conta corrente.");

            var contaAlvo = await ResolverContaAlvoAsync(request, contaDoTitular);

            var descricao = string.IsNullOrWhiteSpace(request.Descricao)
                ? (request.Tipo == "C" ? "Depósito" : "Saque")
                : request.Descricao.Trim();

            // Saldo suficiente e conta ativa são garantidos pelo UPDATE condicional dentro da transação.
            return await _movimentacao.MovimentarAsync(
                contaAlvo.Id, request.Tipo, request.Valor, chave, descricao, cancellationToken);
        }

        private async Task<ContaCorrente> ResolverContaAlvoAsync(MovimentarContaCommand request, ContaCorrente contaDoTitular)
        {
            if (request.NumeroConta is null || request.NumeroConta == contaDoTitular.Numero)
                return contaDoTitular;

            // Regra de negócio: dinheiro pode ENTRAR em conta de terceiro (depósito identificado),
            // mas só o titular pode tirar dinheiro da própria conta.
            if (request.Tipo == "D")
                throw new AcessoNegadoException("Saque só pode ser realizado na conta do próprio titular.");

            return await _contas.ObterPorNumeroAsync(request.NumeroConta.Value)
                ?? throw new RecursoNaoEncontradoException("INVALID_ACCOUNT", $"Conta corrente número {request.NumeroConta.Value} não encontrada.");
        }
    }
}
