namespace TargetSistemas.Domain.Comercial;

/// <summary>
/// Relatório executivo consolidando todos os vendedores e totais gerais da operação comercial.
/// </summary>
public sealed record RelatorioComissao
{
    public IReadOnlyList<VendedorConsolidado> Vendedores { get; }
    public decimal TotalGeralVendas { get; }
    public decimal TotalGeralComissoes { get; }
    public int TotalGeralVendasQuantidade { get; }

    public RelatorioComissao(IEnumerable<VendedorConsolidado> vendedores)
    {
        var lista = vendedores.OrderByDescending(v => v.TotalVendas).ToList().AsReadOnly();
        Vendedores = lista;
        TotalGeralVendas = Math.Round(lista.Sum(v => v.TotalVendas), 2, MidpointRounding.AwayFromZero);
        TotalGeralComissoes = Math.Round(lista.Sum(v => v.TotalComissao), 2, MidpointRounding.AwayFromZero);
        TotalGeralVendasQuantidade = lista.Sum(v => v.QuantidadeVendas);
    }
}
