using TargetSistemas.Domain.Comercial;

namespace TargetSistemas.Application.Comercial.Interfaces;

public interface IComissaoService
{
    RelatorioComissao ProcessarVendas(IEnumerable<Venda> vendas);
    RelatorioComissao ProcessarPayloadJson(string json);
}
