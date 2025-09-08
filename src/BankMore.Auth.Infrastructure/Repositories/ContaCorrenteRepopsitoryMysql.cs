using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;
using Dapper;
using System.Data;

namespace BankMore.Auth.Infrastructure.Repositories
{
    public class ContaCorrenteRepositoryMySql : IContaCorrenteRepository
    {
        private readonly IDbConnection _connection;

        public ContaCorrenteRepositoryMySql(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task AdicionarAsync(ContaCorrente conta)
        {
            var sql = @"INSERT INTO contacorrente 
                        (idcontacorrente, numero, nome, ativo, senha, salt, saldo, criadoEm)
                        VALUES (@Id, @Numero, @Nome, @Ativo, @Senha, @Salt, @Saldo, @CriadoEm)";

            await _connection.ExecuteAsync(sql, new
            {
                Id = conta.Id,
                conta.Numero,
                conta.Nome,
                Ativo = conta.Ativo ? 1 : 0,
                conta.Senha,
                conta.Salt,
                conta.Saldo,
                conta.CriadoEm
            });
        }

        public async Task<bool> NumeroExisteAsync(int numero)
        {
            var sql = @"SELECT COUNT(*) FROM contacorrente WHERE numero = @Numero";
            var count = await _connection.ExecuteScalarAsync<int>(sql, new { Numero = numero });
            return count > 0;
        }

        public async Task<ContaCorrente?> ObterPorIdAsync(Guid id)
        {
            var sql = @"SELECT idcontacorrente as Id, numero as Numero, nome as Nome, 
                               ativo as Ativo, senha as Senha, salt as Salt, 
                               saldo as Saldo, criadoEm as CriadoEm, 
                               atualizadoEm as AtualizadoEm
                        FROM contacorrente WHERE idcontacorrente = @Id";
            
            var contaDto = await _connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });
            
            if (contaDto == null) return null;
            
            return new ContaCorrente(
                contaDto.Id,
                contaDto.Numero,
                contaDto.Nome,
                contaDto.Senha,
                contaDto.Salt
            );
        }

        public async Task<ContaCorrente?> ObterPorNumeroAsync(int numero)
        {
            var sql = @"SELECT idcontacorrente as Id, numero as Numero, nome as Nome, 
                               ativo as Ativo, senha as Senha, salt as Salt, 
                               saldo as Saldo, criadoEm as CriadoEm, 
                               atualizadoEm as AtualizadoEm
                        FROM contacorrente WHERE numero = @Numero";
            
            var contaDto = await _connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Numero = numero });
            
            if (contaDto == null) return null;
            
            return new ContaCorrente(
                contaDto.Id,
                contaDto.Numero,
                contaDto.Nome,
                contaDto.Senha,
                contaDto.Salt
            );
        }

        public async Task AtualizarSaldoAsync(Guid idContaCorrente, decimal novoSaldo)
        {
            var sql = @"UPDATE contacorrente SET saldo = @Saldo, atualizadoEm = @AtualizadoEm 
                        WHERE idcontacorrente = @Id";
            await _connection.ExecuteAsync(sql, new { 
                Id = idContaCorrente, 
                Saldo = novoSaldo, 
                AtualizadoEm = DateTime.UtcNow 
            });
        }

        public async Task<bool> ContaEstaAtivaAsync(Guid idContaCorrente)
        {
            var sql = @"SELECT ativo FROM contacorrente WHERE idcontacorrente = @Id";
            var ativo = await _connection.ExecuteScalarAsync<int?>(sql, new { Id = idContaCorrente });
            return ativo == 1;
        }

        public async Task<decimal> ObterSaldoAsync(Guid idConta)
        {
            var sql = @"SELECT saldo FROM contacorrente WHERE idcontacorrente = @Id";
            var saldo = await _connection.ExecuteScalarAsync<decimal?>(sql, new { Id = idConta });
            return saldo ?? 0;
        }

        public async Task AtualizarAsync(ContaCorrente conta)
        {
            var sql = @"UPDATE contacorrente 
                        SET ativo = @Ativo, atualizadoEm = @AtualizadoEm 
                        WHERE idcontacorrente = @Id";
            await _connection.ExecuteAsync(sql, new { 
                Id = conta.Id, 
                Ativo = conta.Ativo ? 1 : 0,
                AtualizadoEm = DateTime.UtcNow
            });
        }

        public async Task<ContaCorrente?> ObterPorDocumentoOuNumeroAsync(string documentoOuNumero)
        {
            // Tenta primeiro como número, depois como documento
            if (int.TryParse(documentoOuNumero, out int numero))
            {
                return await ObterPorNumeroAsync(numero);
            }
            
            // Se não for número, pode ser um documento (CPF, etc.)
            // Por enquanto, retorna null - pode ser implementado conforme necessário
            return null;
        }
    }
}
