using TargetSistemas.Domain.Financeiro;

namespace TargetSistemas.Application.Financeiro.Interfaces;

public interface IFinanceiroService
{
    CalculoMoraResultado CalcularMora(decimal valorOriginal, DateOnly dataVencimento, DateOnly? dataReferencia = null);
}
