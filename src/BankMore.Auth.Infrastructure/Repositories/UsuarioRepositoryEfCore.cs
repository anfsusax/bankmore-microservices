using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;
using BankMore.Auth.Domain.ValueObjects;
using BankMore.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Auth.Infrastructure.Repositories
{
    public class UsuarioRepositoryEfCore : IUsuarioRepository
    {
        private readonly BankMoreDbContext _context;

        public UsuarioRepositoryEfCore(BankMoreDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObterPorCpfAsync(string cpf)
        {
            var cpfClean = cpf.Replace(".", "").Replace("-", "").Trim();
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Cpf == new CPF(cpfClean));
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == new Email(email));
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
