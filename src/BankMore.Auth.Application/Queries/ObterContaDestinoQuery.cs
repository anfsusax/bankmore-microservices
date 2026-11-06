using BankMore.Auth.Domain.Exceptions;
using BankMore.Auth.Domain.Repositories;
using MediatR;

namespace BankMore.Auth.Application.Queries
{
    /// <summary>
    /// "Para quem estou transferindo?" Igual à confirmação do PIX: mostra o titular MASCARADO
    /// e nunca o saldo nem o ID interno da conta de terceiros.
    /// </summary>
    public record ObterContaDestinoQuery(int Numero) : IRequest<ContaDestinoResult>;

    public sealed record ContaDestinoResult(int Numero, string TitularMascarado, bool Ativa);

    public class ObterContaDestinoQueryHandler : IRequestHandler<ObterContaDestinoQuery, ContaDestinoResult>
    {
        private readonly IContaCorrenteRepository _contas;

        public ObterContaDestinoQueryHandler(IContaCorrenteRepository contas) => _contas = contas;

        public async Task<ContaDestinoResult> Handle(ObterContaDestinoQuery request, CancellationToken cancellationToken)
        {
            var conta = await _contas.ObterPorNumeroAsync(request.Numero)
                ?? throw new RecursoNaoEncontradoException("INVALID_ACCOUNT", "Conta não encontrada.");

            return new ContaDestinoResult(conta.Numero, MascararNome(conta.Nome), conta.Ativo);
        }

        /// <summary>"Alex Nunes Feitoza" → "Alex N. F."</summary>
        public static string MascararNome(string nome)
        {
            var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0) return "***";
            return string.Join(' ', partes.Select((p, i) => i == 0 ? p : $"{char.ToUpperInvariant(p[0])}."));
        }
    }
}
