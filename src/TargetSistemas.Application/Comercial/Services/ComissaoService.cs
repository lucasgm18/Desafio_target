using System.Text.Json;
using TargetSistemas.Application.Comercial.DTOs;
using TargetSistemas.Application.Comercial.Interfaces;
using TargetSistemas.Domain.Comercial;
using TargetSistemas.Domain.Comercial.Exceptions;

namespace TargetSistemas.Application.Comercial.Services;

/// <summary>
/// Serviço de consolidação comercial e cálculo de comissões por vendedor.
/// Implementado sem uso de estruturas condicionais 'else', empregando guard clauses e LINQ fluente.
/// </summary>
public sealed class ComissaoService : IComissaoService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public RelatorioComissao ProcessarVendas(IEnumerable<Venda> vendas)
    {
        if (vendas is null)
            throw new ArgumentNullException(nameof(vendas), "A coleção de vendas não pode ser nula.");

        var consolidados = vendas
            .GroupBy(v => v.Vendedor)
            .Select(grupo => new VendedorConsolidado(grupo.Key, grupo))
            .ToList();

        return new RelatorioComissao(consolidados);
    }

    public RelatorioComissao ProcessarPayloadJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new VendaInvalidaException("O payload JSON de vendas não pode ser vazio.");

        var payload = JsonSerializer.Deserialize<VendasPayloadDto>(json, JsonOptions);

        if (payload?.Vendas is null)
            throw new VendaInvalidaException("O payload não contém uma lista de vendas válida.");

        var vendas = payload.Vendas.Select(dto => new Venda(dto.Vendedor, dto.Valor));
        return ProcessarVendas(vendas);
    }
}
