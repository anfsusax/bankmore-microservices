using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;

namespace BankMore.Auth.Infrastructure.Repositories
{
    // Implementação temporária - será substituída quando a migration de Movimento for criada
    public class MovimentoRepositoryStub : IMovimentoRepository
    {
        public Task AdicionarAsync(Movimento movimento)
        {
            throw new NotImplementedException("Aguardando migration da tabela Movimento");
        }

        public Task<decimal> CalcularSaldoAsync(Guid contaId)
        {
            throw new NotImplementedException("Aguardando migration da tabela Movimento");
        }

        public Task<bool> ExisteIdempotenciaAsync(string chaveIdempotencia)
        {
            throw new NotImplementedException("Aguardando migration da tabela Movimento");
        }
    }
}

