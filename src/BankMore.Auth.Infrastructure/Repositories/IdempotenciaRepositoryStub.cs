using BankMore.Auth.Domain.Repositories;

namespace BankMore.Auth.Infrastructure.Repositories
{
    // Implementação temporária - será substituída quando a migration de Idempotencia for criada
    public class IdempotenciaRepositoryStub : IIdempotenciaRepository
    {
        public Task<bool> ExisteAsync(string chaveIdempotencia)
        {
            throw new NotImplementedException("Aguardando migration da tabela Idempotencia");
        }

        public Task SalvarAsync(string chaveIdempotencia, string resultado)
        {
            throw new NotImplementedException("Aguardando migration da tabela Idempotencia");
        }
    }
}

