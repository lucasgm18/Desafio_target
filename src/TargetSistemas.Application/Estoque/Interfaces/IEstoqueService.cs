using TargetSistemas.Application.Estoque.DTOs;
using TargetSistemas.Domain.Estoque.Entities;

namespace TargetSistemas.Application.Estoque.Interfaces;

public interface IEstoqueService
{
    ResultadoMovimentacaoDto Movimentar(RequisicaoMovimentacaoDto requisicao);
    IReadOnlyList<Produto> ObterCatalogo();
    Produto ObterProduto(int codigoProduto);
    IReadOnlyList<MovimentacaoEstoque> ObterHistoricoMovimentacoes();
    void ReinicializarEstoque();
}
