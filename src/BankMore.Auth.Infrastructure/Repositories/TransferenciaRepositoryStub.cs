using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;

namespace BankMore.Auth.Infrastructure.Repositories
{
    // Implementação temporária - será substituída quando a migration de Transferencia for criada
    public class TransferenciaRepositoryStub : ITransferenciaRepository
    {
        public Task AdicionarAsync(Transferencia transferencia)
        {
            throw new NotImplementedException("Aguardando migration da tabela Transferencia");
        }
    }
}

