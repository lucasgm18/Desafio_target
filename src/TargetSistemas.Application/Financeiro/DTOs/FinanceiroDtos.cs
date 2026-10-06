namespace TargetSistemas.Application.Financeiro.DTOs;

public sealed record RequisicaoCalculoMoraDto(
    decimal ValorOriginal,
    DateOnly DataVencimento,
    DateOnly? DataReferencia = null
);
