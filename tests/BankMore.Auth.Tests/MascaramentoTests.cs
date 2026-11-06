using BankMore.Auth.Application.Queries;
using FluentAssertions;

namespace BankMore.Auth.Tests;

public class MascaramentoTests
{
    [Theory]
    [InlineData("Alex Nunes Feitoza", "Alex N. F.")]
    [InlineData("Maria", "Maria")]
    [InlineData("  ana   paula  ", "ana P.")]
    public void Nome_do_titular_de_destino_e_mascarado(string nome, string esperado)
    {
        ObterContaDestinoQueryHandler.MascararNome(nome).Should().Be(esperado);
    }
}
