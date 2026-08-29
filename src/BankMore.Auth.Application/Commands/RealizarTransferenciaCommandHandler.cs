using BankMore.Auth.Domain.Repositories;
using MediatR;

namespace BankMore.Auth.Application.Commands
{
    public class RealizarTransferenciaCommandHandler : IRequestHandler<RealizarTransferenciaCommand, Guid>
    {
        private readonly ITransferenciaFinanceira _transferencias;

        public RealizarTransferenciaCommandHandler(ITransferenciaFinanceira transferencias)
        {
            _transferencias = transferencias;
        }

        public async Task<Guid> Handle(RealizarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            if (request.IdContaOrigem == Guid.Empty)
                throw new ArgumentException("Conta de origem é obrigatória.");
            if (request.IdContaDestino == Guid.Empty)
                throw new ArgumentException("Conta de destino é obrigatória.");
            if (request.IdContaOrigem == request.IdContaDestino)
                throw new ArgumentException("As contas de origem e destino devem ser diferentes.");
            if (request.Valor <= 0)
                throw new ArgumentException("O valor da transferência deve ser maior que zero.");
            if (string.IsNullOrWhiteSpace(request.ChaveIdempotencia))
                throw new ArgumentException("Chave de idempotência é obrigatória.");

            var idTransferencia = await _transferencias.ProcessarAsync(
                request.IdContaOrigem,
                request.IdContaDestino,
                request.Valor,
                request.ChaveIdempotencia,
                cancellationToken);

            return idTransferencia ?? Guid.Empty;
        }
    }
}
