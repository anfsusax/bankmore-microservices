using BankMore.Auth.Application.Commands;
using BankMore.Auth.Domain.Repositories;
using FluentAssertions;

namespace BankMore.Auth.Tests;

public class RealizarTransferenciaCommandHandlerTests
{
    [Fact]
    public async Task Deve_Encaminhar_Transferencia_Valida_Para_Processador_Transacional()
    {
        var origem = Guid.NewGuid();
        var destino = Guid.NewGuid();
        var idTransferencia = Guid.NewGuid();
        var processador = new ProcessadorFake(idTransferencia);
        var handler = new RealizarTransferenciaCommandHandler(processador);

        var resultado = await handler.Handle(
            new RealizarTransferenciaCommand(origem, destino, 75m, "compra-123"),
            CancellationToken.None);

        resultado.Should().Be(idTransferencia);
        processador.ChaveRecebida.Should().Be("compra-123");
        processador.ValorRecebido.Should().Be(75m);
    }

    [Fact]
    public async Task Deve_Indicar_Conflito_Quando_Chave_Ja_Foi_Processada()
    {
        var handler = new RealizarTransferenciaCommandHandler(new ProcessadorFake(null));

        var resultado = await handler.Handle(
            new RealizarTransferenciaCommand(Guid.NewGuid(), Guid.NewGuid(), 10m, "duplicada"),
            CancellationToken.None);

        resultado.Should().Be(Guid.Empty);
    }

    [Fact]
    public async Task Deve_Rejeitar_Transferencia_Entre_A_Mesma_Conta_Sem_Acessar_Infraestrutura()
    {
        var conta = Guid.NewGuid();
        var processador = new ProcessadorFake(Guid.NewGuid());
        var handler = new RealizarTransferenciaCommandHandler(processador);

        var acao = () => handler.Handle(
            new RealizarTransferenciaCommand(conta, conta, 10m, "invalida"),
            CancellationToken.None);

        await acao.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*devem ser diferentes*");
        processador.FoiChamado.Should().BeFalse();
    }

    private sealed class ProcessadorFake : ITransferenciaFinanceira
    {
        private readonly Guid? _resultado;

        public ProcessadorFake(Guid? resultado) => _resultado = resultado;

        public bool FoiChamado { get; private set; }
        public decimal ValorRecebido { get; private set; }
        public string? ChaveRecebida { get; private set; }

        public Task<Guid?> ProcessarAsync(Guid idContaOrigem, Guid idContaDestino, decimal valor,
            string chaveIdempotencia, CancellationToken cancellationToken = default)
        {
            FoiChamado = true;
            ValorRecebido = valor;
            ChaveRecebida = chaveIdempotencia;
            return Task.FromResult(_resultado);
        }
    }
}
