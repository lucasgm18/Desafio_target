using TargetSistemas.Domain.Comercial.Exceptions;

namespace TargetSistemas.Domain.Comercial;

/// <summary>
/// Representa uma venda individual realizada por um vendedor comercial.
/// Objeto imutável com cálculo autônomo de alíquota e valor de comissão via pattern matching.
/// </summary>
public readonly record struct Venda
{
    public string Vendedor { get; }
    public decimal Valor { get; }

    public Venda(string vendedor, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(vendedor))
            throw new VendaInvalidaException("O nome do vendedor não pode ser vazio ou nulo.");

        if (valor < 0)
            throw new VendaInvalidaException("O valor da venda não pode ser negativo.");

        Vendedor = vendedor.Trim();
        Valor = valor;
    }

    /// <summary>
    /// Alíquota de comissão determinada pela faixa do valor da venda:
    /// - Menor que R$ 100,00: 0% (0.00m)
    /// - De R$ 100,00 até R$ 499,99: 1% (0.01m)
    /// - A partir de R$ 500,00: 5% (0.05m)
    /// </summary>
    public decimal TaxaComissao => Valor switch
    {
        < 100.00m => 0.00m,
        < 500.00m => 0.01m,
        _ => 0.05m
    };

    /// <summary>
    /// Valor monetário da comissão calculado e arredondado para 2 casas decimais.
    /// </summary>
    public decimal ValorComissao => Math.Round(Valor * TaxaComissao, 2, MidpointRounding.AwayFromZero);
}
