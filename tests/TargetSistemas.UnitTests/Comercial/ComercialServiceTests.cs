using TargetSistemas.Application.Comercial.Services;
using TargetSistemas.Application.Common;
using TargetSistemas.Domain.Comercial;
using TargetSistemas.Domain.Comercial.Exceptions;
using Xunit;

namespace TargetSistemas.UnitTests.Comercial;

public class ComercialServiceTests
{
    private readonly ComissaoService _service = new();

    [Theory]
    [InlineData(0.00, 0.00, 0.00)]
    [InlineData(50.00, 0.00, 0.00)]
    [InlineData(99.99, 0.00, 0.00)]
    public void Venda_AbaixoDe100Reais_DevePossuirZeroComissao(decimal valor, decimal taxaEsperada, decimal comissaoEsperada)
    {
        var venda = new Venda("Vendedor Teste", valor);

        Assert.Equal(taxaEsperada, venda.TaxaComissao);
        Assert.Equal(comissaoEsperada, venda.ValorComissao);
    }

    [Theory]
    [InlineData(100.00, 0.01, 1.00)]
    [InlineData(250.30, 0.01, 2.50)]
    [InlineData(480.75, 0.01, 4.81)]
    [InlineData(499.99, 0.01, 5.00)]
    public void Venda_Entre100E499_99_DevePossuirUmPorCentoComissao(decimal valor, decimal taxaEsperada, decimal comissaoEsperada)
    {
        var venda = new Venda("Vendedor Teste", valor);

        Assert.Equal(taxaEsperada, venda.TaxaComissao);
        Assert.Equal(comissaoEsperada, venda.ValorComissao);
    }

    [Theory]
    [InlineData(500.00, 0.05, 25.00)]
    [InlineData(1000.00, 0.05, 50.00)]
    [InlineData(1200.50, 0.05, 60.03)]
    [InlineData(1800.00, 0.05, 90.00)]
    public void Venda_MaiorOuIgualA500_DevePossuirCincoPorCentoComissao(decimal valor, decimal taxaEsperada, decimal comissaoEsperada)
    {
        var venda = new Venda("Vendedor Teste", valor);

        Assert.Equal(taxaEsperada, venda.TaxaComissao);
        Assert.Equal(comissaoEsperada, venda.ValorComissao);
    }

    [Fact]
    public void Venda_ComValorNegativo_DeveLancarExcecao()
    {
        Assert.Throws<VendaInvalidaException>(() => new Venda("Carlos", -10.00m));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Venda_ComNomeVendedorVazio_DeveLancarExcecao(string vendedorInvalido)
    {
        Assert.Throws<VendaInvalidaException>(() => new Venda(vendedorInvalido, 150.00m));
    }

    [Fact]
    public void ProcessarPayloadOficial_DeveConsolidarTodosOsVendedoresCorretamente()
    {
        var relatorio = _service.ProcessarPayloadJson(PayloadsConstantes.VendasJson);

        Assert.NotNull(relatorio);
        Assert.Equal(4, relatorio.Vendedores.Count);
        Assert.Equal(36, relatorio.TotalGeralVendasQuantidade);

        var joao = relatorio.Vendedores.First(v => v.Vendedor == "João Silva");
        Assert.Equal(10, joao.QuantidadeVendas);
        Assert.Equal(10754.70m, joao.TotalVendas);
        Assert.Equal(495.69m, joao.TotalComissao);

        var maria = relatorio.Vendedores.First(v => v.Vendedor == "Maria Souza");
        Assert.Equal(9, maria.QuantidadeVendas);
        Assert.Equal(9874.30m, maria.TotalVendas);
        Assert.Equal(465.96m, maria.TotalComissao);

        var carlos = relatorio.Vendedores.First(v => v.Vendedor == "Carlos Oliveira");
        Assert.Equal(8, carlos.QuantidadeVendas);
        Assert.Equal(7928.35m, carlos.TotalVendas);
        Assert.Equal(379.38m, carlos.TotalComissao);

        var ana = relatorio.Vendedores.First(v => v.Vendedor == "Ana Lima");
        Assert.Equal(9, ana.QuantidadeVendas);
        Assert.Equal(8763.95m, ana.TotalVendas);
        Assert.Equal(404.99m, ana.TotalComissao);

        Assert.Equal(37321.30m, relatorio.TotalGeralVendas);
        Assert.Equal(1746.02m, relatorio.TotalGeralComissoes);
    }
}
