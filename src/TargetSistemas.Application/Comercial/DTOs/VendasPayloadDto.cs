using System.Text.Json.Serialization;

namespace TargetSistemas.Application.Comercial.DTOs;

public sealed record VendaDto(
    [property: JsonPropertyName("vendedor")] string Vendedor,
    [property: JsonPropertyName("valor")] decimal Valor
);

public sealed record VendasPayloadDto(
    [property: JsonPropertyName("vendas")] IReadOnlyList<VendaDto> Vendas
);
