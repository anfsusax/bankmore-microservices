using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BankMore.Auth.Application.Commands
{
    public class MovimentarContaCommandHandler : IRequestHandler<MovimentarContaCommand, Unit>
    {
        private readonly IContaCorrenteRepository _contaRepo;
        private readonly IMovimentoRepository _movimentoRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MovimentarContaCommandHandler(
            IContaCorrenteRepository contaRepo,
            IMovimentoRepository movimentoRepo,
            IHttpContextAccessor httpContextAccessor)
        {
            _contaRepo = contaRepo;
            _movimentoRepo = movimentoRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(MovimentarContaCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
            if (userId is null)
                throw new UnauthorizedAccessException("Usuário não autenticado ou token inválido.");

            var idempotenteExiste = await _movimentoRepo.ExisteIdempotenciaAsync(request.ChaveIdempotencia);
            if (idempotenteExiste)
                return Unit.Value;

            ContaCorrente? conta = null;
            if (request.NumeroConta.HasValue)
            {
                conta = await _contaRepo.ObterPorNumeroAsync(request.NumeroConta.Value);
            }
            else if (Guid.TryParse(userId, out var idContaGuid))
            {
                conta = await _contaRepo.ObterPorIdAsync(idContaGuid);
            }

            if (conta is null)
                throw new InvalidOperationException(request.NumeroConta.HasValue 
                    ? $"Conta corrente número {request.NumeroConta.Value} não encontrada." 
                    : "Conta corrente não encontrada.");

            if (!conta.Ativo)
                throw new InvalidOperationException("Conta corrente inativa.");

            if (request.Valor <= 0)
                throw new InvalidOperationException("O valor da operação deve ser maior que zero.");

            if (request.Tipo != "C" && request.Tipo != "D")
                throw new InvalidOperationException("Tipo de movimentação inválido. Utilize 'C' (Crédito) ou 'D' (Débito).");

            if (request.Tipo == "D" && conta.Saldo < request.Valor)
                throw new InvalidOperationException($"Saldo insuficiente para realizar o saque. Saldo atual: R$ {conta.Saldo:N2}, valor solicitado: R$ {request.Valor:N2}.");

            Movimento movimento;
            if (request.Tipo == "C")
            {
                movimento = Movimento.CriarCredito(conta.Id, request.Valor, request.ChaveIdempotencia, request.Descricao ?? "Depósito");
            }
            else
            {
                movimento = Movimento.CriarDebito(conta.Id, request.Valor, request.ChaveIdempotencia, request.Descricao ?? "Saque");
            }

            await _movimentoRepo.AdicionarAsync(movimento);

            var novoSaldo = await _movimentoRepo.CalcularSaldoAsync(conta.Id);
            await _contaRepo.AtualizarSaldoAsync(conta.Id, novoSaldo);

            return Unit.Value;
        }
    }
}
