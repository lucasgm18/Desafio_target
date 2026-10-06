namespace TargetSistemas.Domain.Comercial;

/// <summary>
/// Representa o resumo consolidado de desempenho e comissões de um vendedor.
/// </summary>
public sealed record VendedorConsolidado
{
    public string Vendedor { get; }
    public int QuantidadeVendas { get; }
    public decimal TotalVendas { get; }
    public decimal TotalComissao { get; }
    public IReadOnlyList<Venda> Vendas { get; }

    public VendedorConsolidado(string vendedor, IEnumerable<Venda> vendas)
    {
        Vendedor = vendedor;
        var listaVendas = vendas.ToList().AsReadOnly();
        Vendas = listaVendas;
        QuantidadeVendas = listaVendas.Count;
        TotalVendas = Math.Round(listaVendas.Sum(v => v.Valor), 2, MidpointRounding.AwayFromZero);
        TotalComissao = Math.Round(listaVendas.Sum(v => v.ValorComissao), 2, MidpointRounding.AwayFromZero);
    }
}
