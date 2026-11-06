using BankMore.Auth.Application.Commands;
using BankMore.Auth.Domain.Exceptions;
using BankMore.Auth.Tests.Fakes;
using FluentAssertions;

namespace BankMore.Auth.Tests;

public class RealizarTransferenciaCommandHandlerTests
{
    private readonly Guid _alex = Guid.NewGuid();
    private readonly Guid _maria = Guid.NewGuid();
    private readonly ContaRepositoryFake _contas = new();
    private readonly TransferenciaFinanceiraFake _processador = new();

    private RealizarTransferenciaCommandHandler CriarHandler(Guid? usuarioLogado) =>
        new(_processador, _contas, new UsuarioAtualFake(usuarioLogado));

    [Fact]
    public async Task Origem_sempre_e_a_conta_do_usuario_autenticado()
    {
        var contaAlex = _contas.Adicionar(_alex, 1001);
        var contaMaria = _contas.Adicionar(_maria, 2002);

        await CriarHandler(_alex).Handle(new RealizarTransferenciaCommand(2002, 75m, "compra-123"), CancellationToken.None);

        _processador.Origem.Should().Be(contaAlex.Id);
        _processador.Destino.Should().Be(contaMaria.Id);
        _processador.Valor.Should().Be(75m);
    }

    [Fact]
    public async Task Chave_de_idempotencia_e_escopada_ao_usuario()
    {
        _contas.Adicionar(_alex, 1001);
        _contas.Adicionar(_maria, 2002);

        await CriarHandler(_alex).Handle(new RealizarTransferenciaCommand(2002, 10m, "abc"), CancellationToken.None);

        _processador.Chave.Should().Be($"{_alex:N}:abc");
    }

    [Fact]
    public async Task Requisicao_sem_usuario_autenticado_e_recusada_sem_tocar_na_infraestrutura()
    {
        _contas.Adicionar(_maria, 2002);

        var acao = () => CriarHandler(usuarioLogado: null)
            .Handle(new RealizarTransferenciaCommand(2002, 10m, "x"), CancellationToken.None);

        await acao.Should().ThrowAsync<UnauthorizedAccessException>();
        _processador.FoiChamado.Should().BeFalse();
    }

    [Fact]
    public async Task Usuario_sem_conta_recebe_nao_encontrado()
    {
        _contas.Adicionar(_maria, 2002);

        var acao = () => CriarHandler(_alex).Handle(new RealizarTransferenciaCommand(2002, 10m, "x"), CancellationToken.None);

        await acao.Should().ThrowAsync<RecursoNaoEncontradoException>();
        _processador.FoiChamado.Should().BeFalse();
    }

    [Fact]
    public async Task Rejeita_transferencia_para_a_propria_conta()
    {
        _contas.Adicionar(_alex, 1001);

        var acao = () => CriarHandler(_alex).Handle(new RealizarTransferenciaCommand(1001, 10m, "x"), CancellationToken.None);

        await acao.Should().ThrowAsync<ArgumentException>().WithMessage("*devem ser diferentes*");
        _processador.FoiChamado.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Rejeita_valor_nao_positivo(decimal valor)
    {
        var acao = () => CriarHandler(_alex).Handle(new RealizarTransferenciaCommand(2002, valor, "x"), CancellationToken.None);

        await acao.Should().ThrowAsync<ArgumentException>();
        _processador.FoiChamado.Should().BeFalse();
    }

    [Fact]
    public async Task Replay_e_repassado_ao_chamador()
    {
        _contas.Adicionar(_alex, 1001);
        _contas.Adicionar(_maria, 2002);
        var original = Guid.NewGuid();
        _processador.Resultado = new(original, Replay: true);

        var resultado = await CriarHandler(_alex).Handle(new RealizarTransferenciaCommand(2002, 10m, "dup"), CancellationToken.None);

        resultado.Id.Should().Be(original);
        resultado.Replay.Should().BeTrue();
    }
}
