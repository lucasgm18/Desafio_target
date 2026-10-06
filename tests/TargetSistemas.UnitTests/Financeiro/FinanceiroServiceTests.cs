using TargetSistemas.Application.Financeiro.Services;
using TargetSistemas.Domain.Financeiro.Exceptions;
using Xunit;

namespace TargetSistemas.UnitTests.Financeiro;

public class FinanceiroServiceTests
{
    private readonly FinanceiroService _service = new();

    [Fact]
    public void CalcularMora_TituloEmDia_NaoDeveSofrerAcrescimo()
    {
        var hoje = new DateOnly(2026, 10, 5);
        var vencimento = hoje;

        var resultado = _service.CalcularMora(1000.00m, vencimento, hoje);

        Assert.Equal(1000.00m, resultado.ValorOriginal);
        Assert.Equal(0, resultado.DiasAtraso);
        Assert.Equal(0m, resultado.ValorMulta);
        Assert.Equal(0m, resultado.PercentualTotalMora);
        Assert.Equal(1000.00m, resultado.ValorTotal);
        Assert.False(resultado.PossuiAtraso);
    }

    [Fact]
    public void CalcularMora_TituloVencimentoFuturo_NaoDeveSofrerAcrescimo()
    {
        var referencia = new DateOnly(2026, 10, 5);
        var vencimentoFuturo = new DateOnly(2026, 10, 20);

        var resultado = _service.CalcularMora(2500.00m, vencimentoFuturo, referencia);

        Assert.Equal(2500.00m, resultado.ValorOriginal);
        Assert.Equal(0, resultado.DiasAtraso);
        Assert.Equal(0m, resultado.ValorMulta);
        Assert.Equal(2500.00m, resultado.ValorTotal);
        Assert.False(resultado.PossuiAtraso);
    }

    [Fact]
    public void CalcularMora_ComUmDiaDeAtraso_DeveAplicarDoisMeioPorCento()
    {
        var vencimento = new DateOnly(2026, 10, 4);
        var referencia = new DateOnly(2026, 10, 5);

        var resultado = _service.CalcularMora(1000.00m, vencimento, referencia);

        Assert.Equal(1, resultado.DiasAtraso);
        Assert.Equal(2.5m, resultado.TaxaDiariaPercentual);
        Assert.Equal(2.5m, resultado.PercentualTotalMora);
        Assert.Equal(25.00m, resultado.ValorMulta);
        Assert.Equal(1025.00m, resultado.ValorTotal);
        Assert.True(resultado.PossuiAtraso);
    }

    [Fact]
    public void CalcularMora_ComQuatroDiasDeAtrasoParaMilReais_DeveAplicarCemReaisDeMulta()
    {
        var referencia = new DateOnly(2026, 10, 5);
        var vencimento = referencia.AddDays(-4); // 4 dias de atraso

        var resultado = _service.CalcularMora(1000.00m, vencimento, referencia);

        Assert.Equal(4, resultado.DiasAtraso);
        Assert.Equal(10.0m, resultado.PercentualTotalMora); // 4 * 2.5% = 10%
        Assert.Equal(100.00m, resultado.ValorMulta); // 1.000 * 10% = 100.00
        Assert.Equal(1100.00m, resultado.ValorTotal);
        Assert.True(resultado.PossuiAtraso);
    }

    [Fact]
    public void CalcularMora_ComDezDiasDeAtraso_DeveAplicarVinteCincoPorCento()
    {
        var vencimento = new DateOnly(2026, 9, 25);
        var referencia = new DateOnly(2026, 10, 5);

        var resultado = _service.CalcularMora(1000.00m, vencimento, referencia);

        Assert.Equal(10, resultado.DiasAtraso);
        Assert.Equal(25.0m, resultado.PercentualTotalMora);
        Assert.Equal(250.00m, resultado.ValorMulta);
        Assert.Equal(1250.00m, resultado.ValorTotal);
        Assert.True(resultado.PossuiAtraso);
    }

    [Fact]
    public void CalcularMora_ComTrintaDiasDeAtraso_DeveCalcularCorretamente()
    {
        var vencimento = new DateOnly(2026, 9, 5);
        var referencia = new DateOnly(2026, 10, 5);

        var resultado = _service.CalcularMora(500.00m, vencimento, referencia);

        Assert.Equal(30, resultado.DiasAtraso);
        Assert.Equal(75.0m, resultado.PercentualTotalMora);
        Assert.Equal(375.00m, resultado.ValorMulta);
        Assert.Equal(875.00m, resultado.ValorTotal);
    }

    [Fact]
    public void CalcularMora_ComCentavosEArredondamento_DeveArredondarParaDuasCasasDecimais()
    {
        var vencimento = new DateOnly(2026, 10, 2);
        var referencia = new DateOnly(2026, 10, 5); // 3 dias = 7.5%

        var resultado = _service.CalcularMora(123.45m, vencimento, referencia);

        Assert.Equal(3, resultado.DiasAtraso);
        Assert.Equal(7.5m, resultado.PercentualTotalMora);
        // 123.45 * 0.075 = 9.25875 => 9.26
        Assert.Equal(9.26m, resultado.ValorMulta);
        Assert.Equal(132.71m, resultado.ValorTotal);
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-50.00)]
    public void CalcularMora_ValorInvalido_DeveLancarExcecao(decimal valorInvalido)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        Assert.Throws<FinanceiroException>(() => _service.CalcularMora(valorInvalido, hoje));
    }
}
