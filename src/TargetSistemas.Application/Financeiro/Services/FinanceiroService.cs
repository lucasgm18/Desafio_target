using TargetSistemas.Application.Financeiro.Interfaces;
using TargetSistemas.Domain.Financeiro;
using TargetSistemas.Domain.Financeiro.Exceptions;

namespace TargetSistemas.Application.Financeiro.Services;

/// <summary>
/// Serviço de liquidação e cálculo de encargos financeiros (Mora e Multa).
/// Aplica alíquota diária de 2,5% por dia corrido de atraso.
/// Títulos em dia ou futuros não recebem acréscimo.
/// Implementado sem o uso de 'else', priorizando guard clauses e transparência nos cálculos.
/// </summary>
public sealed class FinanceiroService : IFinanceiroService
{
    public const decimal TaxaDiariaPadrao = 2.5m; // 2,5% ao dia corrido
    private const decimal FatorMultiplicadorTaxa = TaxaDiariaPadrao / 100m; // 0.025m

    public CalculoMoraResultado CalcularMora(decimal valorOriginal, DateOnly dataVencimento, DateOnly? dataReferencia = null)
    {
        if (valorOriginal <= 0)
            throw new FinanceiroException("O valor original do título deve ser maior que zero.");

        var referencia = dataReferencia ?? DateOnly.FromDateTime(DateTime.Today);
        var diferencaDias = referencia.DayNumber - dataVencimento.DayNumber;

        // Títulos em dia ou com vencimento futuro
        if (diferencaDias <= 0)
        {
            return new CalculoMoraResultado(
                ValorOriginal: valorOriginal,
                DataVencimento: dataVencimento,
                DataReferencia: referencia,
                DiasAtraso: 0,
                TaxaDiariaPercentual: TaxaDiariaPadrao,
                PercentualTotalMora: 0m,
                ValorMulta: 0m,
                ValorTotal: valorOriginal,
                PossuiAtraso: false
            );
        }

        var diasAtraso = diferencaDias;
        var percentualTotalMora = diasAtraso * TaxaDiariaPadrao;
        var fatorAcumulado = diasAtraso * FatorMultiplicadorTaxa;
        var valorMulta = Math.Round(valorOriginal * fatorAcumulado, 2, MidpointRounding.AwayFromZero);
        var valorTotal = valorOriginal + valorMulta;

        return new CalculoMoraResultado(
            ValorOriginal: valorOriginal,
            DataVencimento: dataVencimento,
            DataReferencia: referencia,
            DiasAtraso: diasAtraso,
            TaxaDiariaPercentual: TaxaDiariaPadrao,
            PercentualTotalMora: percentualTotalMora,
            ValorMulta: valorMulta,
            ValorTotal: valorTotal,
            PossuiAtraso: true
        );
    }
}
