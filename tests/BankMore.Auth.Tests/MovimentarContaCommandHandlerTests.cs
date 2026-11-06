using BankMore.Auth.Application.Commands;
using BankMore.Auth.Domain.Exceptions;
using BankMore.Auth.Tests.Fakes;
using FluentAssertions;

namespace BankMore.Auth.Tests;

public class MovimentarContaCommandHandlerTests
{
    private readonly Guid _alex = Guid.NewGuid();
    private readonly Guid _maria = Guid.NewGuid();
    private readonly ContaRepositoryFake _contas = new();
    private readonly MovimentacaoFinanceiraFake _movimentacao = new();

    private MovimentarContaCommandHandler CriarHandler(Guid? usuarioLogado) =>
        new(_contas, _movimentacao, new UsuarioAtualFake(usuarioLogado));

    [Fact]
    public async Task Saque_em_conta_de_terceiro_e_proibido()
    {
        _contas.Adicionar(_alex, 1001);
        _contas.Adicionar(_maria, 2002);

        var acao = () => CriarHandler(_alex)
            .Handle(new MovimentarContaCommand("D", 50m, "k1", NumeroConta: 2002), CancellationToken.None);

        await acao.Should().ThrowAsync<AcessoNegadoException>();
        _movimentacao.FoiChamado.Should().BeFalse();
    }

    [Fact]
    public async Task Deposito_em_conta_de_terceiro_e_permitido()
    {
        _contas.Adicionar(_alex, 1001);
        var contaMaria = _contas.Adicionar(_maria, 2002);

        await CriarHandler(_alex).Handle(new MovimentarContaCommand("C", 50m, "k1", NumeroConta: 2002), CancellationToken.None);

        _movimentacao.ContaMovimentada.Should().Be(contaMaria.Id);
        _movimentacao.Tipo.Should().Be("C");
    }

    [Fact]
    public async Task Sem_numero_de_conta_movimenta_a_conta_do_titular()
    {
        var contaAlex = _contas.Adicionar(_alex, 1001);

        await CriarHandler(_alex).Handle(new MovimentarContaCommand("D", 20m, "k1"), CancellationToken.None);

        _movimentacao.ContaMovimentada.Should().Be(contaAlex.Id);
        _movimentacao.Chave.Should().Be($"{_alex:N}:k1");
    }

    [Fact]
    public async Task Tipo_invalido_e_recusado()
    {
        _contas.Adicionar(_alex, 1001);

        var acao = () => CriarHandler(_alex).Handle(new MovimentarContaCommand("X", 20m, "k1"), CancellationToken.None);

        await acao.Should().ThrowAsync<ArgumentException>();
        _movimentacao.FoiChamado.Should().BeFalse();
    }

    [Fact]
    public async Task Chave_de_idempotencia_maior_que_o_limite_e_recusada()
    {
        _contas.Adicionar(_alex, 1001);

        var acao = () => CriarHandler(_alex)
            .Handle(new MovimentarContaCommand("C", 20m, new string('k', 129)), CancellationToken.None);

        await acao.Should().ThrowAsync<ArgumentException>();
    }
}
