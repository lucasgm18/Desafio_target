using TargetSistemas.Domain.Estoque.Entities;

namespace TargetSistemas.Application.Estoque.Interfaces;

public interface IEstoqueRepository
{
    IReadOnlyList<Produto> ObterTodos();
    Produto? ObterPorCodigo(int codigo);
    void Atualizar(Produto produto);
    void RegistrarMovimentacao(MovimentacaoEstoque movimentacao);
    IReadOnlyList<MovimentacaoEstoque> ObterHistorico();
    void ResetarParaEstadoInicial();
}
