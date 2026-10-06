using System.Collections.Concurrent;
using TargetSistemas.Application.Estoque.Interfaces;
using TargetSistemas.Domain.Estoque.Entities;

namespace TargetSistemas.Application.Estoque.Repositories;

/// <summary>
/// Repositório em memória com carga dos dados iniciais do inventário.
/// Thread-safe e sem uso da palavra-chave 'else'.
/// </summary>
public sealed class InMemoryEstoqueRepository : IEstoqueRepository
{
    private readonly ConcurrentDictionary<int, Produto> _produtos = new();
    private readonly ConcurrentQueue<MovimentacaoEstoque> _historico = new();

    private static readonly (int Codigo, string Descricao, int Saldo)[] ProdutosIniciais =
    [
        (101, "Caneta Azul", 150),
        (102, "Caderno Universitário", 75),
        (103, "Borracha Branca", 200),
        (104, "Lápis Preto HB", 320),
        (105, "Marcador de Texto Amarelo", 90)
    ];

    public InMemoryEstoqueRepository()
    {
        CarregarProdutosIniciais();
    }

    public IReadOnlyList<Produto> ObterTodos()
    {
        return _produtos.Values.OrderBy(p => p.Codigo).ToList().AsReadOnly();
    }

    public Produto? ObterPorCodigo(int codigo)
    {
        _produtos.TryGetValue(codigo, out var produto);
        return produto;
    }

    public void Atualizar(Produto produto)
    {
        _produtos[produto.Codigo] = produto;
    }

    public void RegistrarMovimentacao(MovimentacaoEstoque movimentacao)
    {
        _historico.Enqueue(movimentacao);
    }

    public IReadOnlyList<MovimentacaoEstoque> ObterHistorico()
    {
        return _historico.OrderByDescending(m => m.DataHora).ToList().AsReadOnly();
    }

    public void ResetarParaEstadoInicial()
    {
        _produtos.Clear();
        _historico.Clear();
        CarregarProdutosIniciais();
    }

    private void CarregarProdutosIniciais()
    {
        foreach (var (codigo, descricao, saldo) in ProdutosIniciais)
        {
            _produtos[codigo] = new Produto(codigo, descricao, saldo);
        }
    }
}
