using BankMore.Auth.Application.Abstractions;
using BankMore.Auth.Domain.Exceptions;
using BankMore.Auth.Domain.Repositories;
using MediatR;

namespace BankMore.Auth.Application.Commands
{
    public class RealizarTransferenciaCommandHandler : IRequestHandler<RealizarTransferenciaCommand, ResultadoOperacaoFinanceira>
    {
        private readonly ITransferenciaFinanceira _transferencias;
        private readonly IContaCorrenteRepository _contas;
        private readonly IUsuarioAtual _usuarioAtual;

        public RealizarTransferenciaCommandHandler(
            ITransferenciaFinanceira transferencias,
            IContaCorrenteRepository contas,
            IUsuarioAtual usuarioAtual)
        {
            _transferencias = transferencias;
            _contas = contas;
            _usuarioAtual = usuarioAtual;
        }

        public async Task<ResultadoOperacaoFinanceira> Handle(RealizarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            if (request.Valor <= 0)
                throw new ArgumentException("O valor da transferência deve ser maior que zero.");
            if (request.NumeroContaDestino <= 0)
                throw new ArgumentException("Conta de destino é obrigatória.");

            var idUsuario = _usuarioAtual.ObterIdUsuario();
            var chave = ChaveIdempotencia.Escopar(idUsuario, request.ChaveIdempotencia);

            var origem = await _contas.ObterPorUsuarioAsync(idUsuario)
                ?? throw new RecursoNaoEncontradoException("INVALID_ACCOUNT", "O usuário autenticado não possui conta corrente.");

            var destino = await _contas.ObterPorNumeroAsync(request.NumeroContaDestino)
                ?? throw new RecursoNaoEncontradoException("INVALID_DESTINATION", "Conta de destino não encontrada.");

            if (origem.Id == destino.Id)
                throw new ArgumentException("As contas de origem e destino devem ser diferentes.");

            // Saldo e situação das contas são checados DENTRO da transação (UPDATE condicional),
            // nunca aqui: checar antes e debitar depois é exatamente a race condition do double-spend.
            return await _transferencias.ProcessarAsync(origem.Id, destino.Id, request.Valor, chave, cancellationToken);
        }
    }
}
