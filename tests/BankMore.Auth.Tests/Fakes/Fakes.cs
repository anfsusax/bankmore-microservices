using BankMore.Auth.Application.Abstractions;
using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;

namespace BankMore.Auth.Tests.Fakes;

internal sealed class UsuarioAtualFake : IUsuarioAtual
{
    private readonly Guid? _id;
    public UsuarioAtualFake(Guid? id) => _id = id;
    public Guid ObterIdUsuario() => _id ?? throw new UnauthorizedAccessException("Usuário não autenticado.");
}

internal sealed class ContaRepositoryFake : IContaCorrenteRepository
{
    private readonly List<ContaCorrente> _contas = new();

    public ContaCorrente Adicionar(Guid idUsuario, int numero, string nome = "Titular Teste")
    {
        var conta = ContaCorrente.Criar(idUsuario, numero, nome, "hash", "salt");
        _contas.Add(conta);
        return conta;
    }

    public Task AdicionarAsync(ContaCorrente conta) { _contas.Add(conta); return Task.CompletedTask; }
    public Task<bool> NumeroExisteAsync(int numero) => Task.FromResult(_contas.Any(c => c.Numero == numero));
    public Task<ContaCorrente?> ObterPorIdAsync(Guid id) => Task.FromResult(_contas.FirstOrDefault(c => c.Id == id));
    public Task<ContaCorrente?> ObterPorNumeroAsync(int numero) => Task.FromResult(_contas.FirstOrDefault(c => c.Numero == numero));
    public Task<ContaCorrente?> ObterPorUsuarioAsync(Guid idUsuario) => Task.FromResult(_contas.FirstOrDefault(c => c.PertenceA(idUsuario)));
    public Task AtualizarSaldoAsync(Guid idConta, decimal novoSaldo) => throw new NotSupportedException();
    public Task<bool> ContaEstaAtivaAsync(Guid idConta) => Task.FromResult(_contas.Any(c => c.Id == idConta && c.Ativo));
    public Task<decimal> ObterSaldoAsync(Guid idConta) => Task.FromResult(_contas.First(c => c.Id == idConta).Saldo);
    public Task AtualizarAsync(ContaCorrente conta) => Task.CompletedTask;
    public Task<ContaCorrente?> ObterPorDocumentoOuNumeroAsync(string documentoOuNumero) => throw new NotSupportedException();
    public Task<ContaCorrente?> ObterPorNomeAsync(string nome) => throw new NotSupportedException();
}

internal sealed class TransferenciaFinanceiraFake : ITransferenciaFinanceira
{
    public bool FoiChamado { get; private set; }
    public Guid Origem { get; private set; }
    public Guid Destino { get; private set; }
    public decimal Valor { get; private set; }
    public string? Chave { get; private set; }
    public ResultadoOperacaoFinanceira Resultado { get; set; } = new(Guid.NewGuid(), false);

    public Task<ResultadoOperacaoFinanceira> ProcessarAsync(Guid idContaOrigem, Guid idContaDestino, decimal valor,
        string chaveIdempotencia, CancellationToken cancellationToken = default)
    {
        FoiChamado = true;
        Origem = idContaOrigem;
        Destino = idContaDestino;
        Valor = valor;
        Chave = chaveIdempotencia;
        return Task.FromResult(Resultado);
    }
}

internal sealed class MovimentacaoFinanceiraFake : IMovimentacaoFinanceira
{
    public bool FoiChamado { get; private set; }
    public Guid ContaMovimentada { get; private set; }
    public string? Tipo { get; private set; }
    public string? Chave { get; private set; }

    public Task<ResultadoOperacaoFinanceira> MovimentarAsync(Guid idConta, string tipo, decimal valor,
        string chaveIdempotencia, string descricao, CancellationToken cancellationToken = default)
    {
        FoiChamado = true;
        ContaMovimentada = idConta;
        Tipo = tipo;
        Chave = chaveIdempotencia;
        return Task.FromResult(new ResultadoOperacaoFinanceira(Guid.NewGuid(), false));
    }
}
