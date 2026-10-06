namespace TargetSistemas.Domain.Financeiro;

/// <summary>
/// DTO imutável contendo o detalhamento completo do cálculo de juros e mora de um título financeiro.
/// </summary>
public sealed record CalculoMoraResultado(
    decimal ValorOriginal,
    DateOnly DataVencimento,
    DateOnly DataReferencia,
    int DiasAtraso,
    decimal TaxaDiariaPercentual,
    decimal PercentualTotalMora,
    decimal ValorMulta,
    decimal ValorTotal,
    bool PossuiAtraso
);
