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
            if (string.IsNullOrWhiteSpace(cpf))
                return null;

            var cpfClean = System.Text.RegularExpressions.Regex.Replace(cpf, @"[^\d]", "");
            if (!CPF.ValidarFormato(cpfClean))
                return null;

            var cpfVo = new CPF(cpfClean);
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Cpf == cpfVo);
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            try
            {
                var emailVo = new Email(email.Trim());
                return await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == emailVo);
            }
            catch (ArgumentException)
            {
                return null;
            }
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
