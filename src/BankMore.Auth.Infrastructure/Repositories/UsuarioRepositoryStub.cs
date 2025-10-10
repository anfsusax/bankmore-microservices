using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;

namespace BankMore.Auth.Infrastructure.Repositories
{
    // Implementação temporária - será substituída quando a migration de Usuario for criada
    public class UsuarioRepositoryStub : IUsuarioRepository
    {
        public Task<Usuario?> ObterPorCpfAsync(string cpf)
        {
            throw new NotImplementedException("Aguardando migration da tabela Usuario");
        }

        public Task<Usuario?> ObterPorEmailAsync(string email)
        {
            throw new NotImplementedException("Aguardando migration da tabela Usuario");
        }

        public Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            throw new NotImplementedException("Aguardando migration da tabela Usuario");
        }

        public Task AdicionarAsync(Usuario usuario)
        {
            throw new NotImplementedException("Aguardando migration da tabela Usuario");
        }

        public Task AtualizarAsync(Usuario usuario)
        {
            throw new NotImplementedException("Aguardando migration da tabela Usuario");
        }
    }
}

