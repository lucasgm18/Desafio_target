using System.Text.Json.Serialization;
using TargetSistemas.Domain.Estoque.Enums;

namespace TargetSistemas.Application.Estoque.DTOs;

public sealed record ItemEstoqueInicialDto(
    [property: JsonPropertyName("codigoProduto")] int CodigoProduto,
    [property: JsonPropertyName("descricaoProduto")] string DescricaoProduto,
    [property: JsonPropertyName("estoque")] int Estoque
);

public sealed record EstoqueInicialPayloadDto(
    [property: JsonPropertyName("estoque")] IReadOnlyList<ItemEstoqueInicialDto> Estoque
);

public sealed record RequisicaoMovimentacaoDto(
    int CodigoProduto,
    TipoMovimentacao Tipo,
    int Quantidade,
    string Motivo
);

public sealed record ResultadoMovimentacaoDto(
    Guid MovimentacaoId,
    int CodigoProduto,
    string DescricaoProduto,
    TipoMovimentacao Tipo,
    int QuantidadeMovimentada,
    int SaldoAnterior,
    int SaldoAtualizado,
    string Motivo,
    DateTimeOffset DataHora
);
