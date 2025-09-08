using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;
using Dapper;
using System.Data;

namespace BankMore.Auth.Infrastructure.Repositories
{
    public class UsuarioRepositoryMySql : IUsuarioRepository
    {
        private readonly IDbConnection _connection;

        public UsuarioRepositoryMySql(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Usuario?> ObterPorCpfAsync(string cpf)
        {
            var sql = @"SELECT id, nome, cpf, email, senhaHash, ativo, criadoEm 
                       FROM usuarios WHERE cpf = @Cpf AND ativo = 1";
            
            var usuarioDto = await _connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Cpf = cpf });
            
            if (usuarioDto == null) return null;
            
            return new Usuario(
                usuarioDto.id,
                usuarioDto.nome,
                new Domain.ValueObjects.CPF(usuarioDto.cpf),
                new Domain.ValueObjects.Email(usuarioDto.email),
                usuarioDto.senhaHash
            );
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            var sql = @"SELECT id, nome, cpf, email, senhaHash, ativo, criadoEm 
                       FROM usuarios WHERE email = @Email AND ativo = 1";
            
            var usuarioDto = await _connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Email = email });
            
            if (usuarioDto == null) return null;
            
            return new Usuario(
                usuarioDto.id,
                usuarioDto.nome,
                new Domain.ValueObjects.CPF(usuarioDto.cpf),
                new Domain.ValueObjects.Email(usuarioDto.email),
                usuarioDto.senhaHash
            );
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            var sql = @"SELECT id, nome, cpf, email, senhaHash, ativo, criadoEm 
                       FROM usuarios WHERE id = @Id AND ativo = 1";
            
            var usuarioDto = await _connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });
            
            if (usuarioDto == null) return null;
            
            return new Usuario(
                usuarioDto.id,
                usuarioDto.nome,
                new Domain.ValueObjects.CPF(usuarioDto.cpf),
                new Domain.ValueObjects.Email(usuarioDto.email),
                usuarioDto.senhaHash
            );
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            var sql = @"INSERT INTO usuarios 
                        (id, nome, cpf, email, senhaHash, ativo, criadoEm)
                        VALUES (@Id, @Nome, @Cpf, @Email, @SenhaHash, @Ativo, @CriadoEm)";

            await _connection.ExecuteAsync(sql, new
            {
                Id = usuario.Id,
                usuario.Nome,
                Cpf = usuario.Cpf.Numero,
                Email = usuario.Email.Endereco,
                SenhaHash = usuario.SenhaHash,
                Ativo = usuario.Ativo ? 1 : 0,
                CriadoEm = usuario.CriadoEm
            });
        }

        public async Task AtualizarAsync(Usuario usuario)
        {
            var sql = @"UPDATE usuarios 
                        SET nome = @Nome, cpf = @Cpf, email = @Email, 
                            senhaHash = @SenhaHash, ativo = @Ativo
                        WHERE id = @Id";

            await _connection.ExecuteAsync(sql, new
            {
                Id = usuario.Id,
                usuario.Nome,
                Cpf = usuario.Cpf.Numero,
                Email = usuario.Email.Endereco,
                SenhaHash = usuario.SenhaHash,
                Ativo = usuario.Ativo ? 1 : 0
            });
        }
    }
}