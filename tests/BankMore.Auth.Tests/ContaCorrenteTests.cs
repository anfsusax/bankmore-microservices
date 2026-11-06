using BankMore.Auth.Domain.Entities;
using FluentAssertions;

namespace BankMore.Auth.Tests;

public class ContaCorrenteTests
{
    [Fact]
    public void Deve_Criar_Conta_Corrente_Valida()
    {
        // Arrange
        var numero = 12345;
        var nome = "João Silva";
        var senhaHash = "hash123";
        var salt = "salt123";

        // Act
        var conta = ContaCorrente.Criar(Guid.NewGuid(), numero, nome, senhaHash, salt);

        // Assert
        conta.Should().NotBeNull();
        conta.Numero.Should().Be(numero);
        conta.Nome.Should().Be(nome);
        conta.Senha.Should().Be(senhaHash);
        conta.Salt.Should().Be(salt);
        conta.Ativo.Should().BeTrue();
        conta.Saldo.Should().Be(0);
        conta.CriadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_Lancar_Excecao_Para_Numero_Invalido(int numeroInvalido)
    {
        // Act & Assert
        Action acao = () => ContaCorrente.Criar(Guid.NewGuid(), numeroInvalido, "Nome", "senha", "salt");
        acao.Should().Throw<ArgumentException>().WithMessage("*maior que zero*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Deve_Lancar_Excecao_Para_Nome_Vazio(string nomeInvalido)
    {
        // Act & Assert
        Action acao = () => ContaCorrente.Criar(Guid.NewGuid(), 123, nomeInvalido, "senha", "salt");
        acao.Should().Throw<ArgumentException>().WithMessage("*obrigatório*");
    }

    [Fact]
    public void Deve_Creditar_Valor_Na_Conta()
    {
        // Arrange
        var conta = ContaCorrente.Criar(Guid.NewGuid(), 123, "Nome", "senha", "salt");
        var valorCredito = 100.50m;

        // Act
        conta.Creditar(valorCredito);

        // Assert
        conta.Saldo.Should().Be(valorCredito);
        conta.AtualizadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Deve_Debitar_Valor_Da_Conta()
    {
        // Arrange
        var conta = ContaCorrente.Criar(Guid.NewGuid(), 123, "Nome", "senha", "salt");
        conta.Creditar(200m);
        var valorDebito = 50m;

        // Act
        conta.Debitar(valorDebito);

        // Assert
        conta.Saldo.Should().Be(150m);
        conta.AtualizadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Deve_Lancar_Excecao_Ao_Debitar_Valor_Maior_Que_Saldo()
    {
        // Arrange
        var conta = ContaCorrente.Criar(Guid.NewGuid(), 123, "Nome", "senha", "salt");
        conta.Creditar(100m);

        // Act & Assert
        Action acao = () => conta.Debitar(150m);
        acao.Should().Throw<InvalidOperationException>().WithMessage("*insuficiente*");
    }

    [Fact]
    public void Deve_Desativar_Conta()
    {
        // Arrange
        var conta = ContaCorrente.Criar(Guid.NewGuid(), 123, "Nome", "senha", "salt");

        // Act
        conta.Desativar();

        // Assert
        conta.Ativo.Should().BeFalse();
        conta.AtualizadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Deve_Verificar_Saldo_Suficiente()
    {
        // Arrange
        var conta = ContaCorrente.Criar(Guid.NewGuid(), 123, "Nome", "senha", "salt");
        conta.Creditar(100m);

        // Act & Assert
        conta.TemSaldoSuficiente(50m).Should().BeTrue();
        conta.TemSaldoSuficiente(100m).Should().BeTrue();
        conta.TemSaldoSuficiente(150m).Should().BeFalse();
    }

    [Fact]
    public void Conta_sem_titular_nao_pode_ser_criada()
    {
        Action acao = () => ContaCorrente.Criar(Guid.Empty, 123, "Nome", "senha", "salt");

        acao.Should().Throw<ArgumentException>().WithMessage("*titular*");
    }

    [Fact]
    public void Conta_pertence_somente_ao_seu_titular()
    {
        var titular = Guid.NewGuid();
        var conta = ContaCorrente.Criar(titular, 123, "Nome", "senha", "salt");

        conta.PertenceA(titular).Should().BeTrue();
        conta.PertenceA(Guid.NewGuid()).Should().BeFalse();
    }
}
