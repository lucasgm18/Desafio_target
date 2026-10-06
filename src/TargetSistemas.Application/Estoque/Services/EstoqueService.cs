using TargetSistemas.Application.Estoque.DTOs;
using TargetSistemas.Application.Estoque.Interfaces;
using TargetSistemas.Domain.Estoque.Entities;
using TargetSistemas.Domain.Estoque.Enums;
using TargetSistemas.Domain.Estoque.Exceptions;

namespace TargetSistemas.Application.Estoque.Services;

/// <summary>
/// Motor de movimentação de mercadorias.
/// Valida integridade do catálogo, restrições de quantidade, motivos de auditoria e saldos negativos.
/// Desenvolvido sob regras estritas de Clean Code e Object Calisthenics (zero 'else').
/// </summary>
public sealed class EstoqueService : IEstoqueService
{
    private readonly IEstoqueRepository _repository;
    private readonly object _syncLock = new();

    public EstoqueService(IEstoqueRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public ResultadoMovimentacaoDto Movimentar(RequisicaoMovimentacaoDto requisicao)
    {
        if (requisicao is null)
            throw new ArgumentNullException(nameof(requisicao), "A requisição de movimentação é obrigatória.");

        if (requisicao.Quantidade <= 0)
            throw new MovimentacaoInvalidaException("A quantidade movimentada deve ser estritamente positiva.");

        if (string.IsNullOrWhiteSpace(requisicao.Motivo))
            throw new MovimentacaoInvalidaException("O motivo/descrição da movimentação é obrigatório para fins de auditoria.");

        lock (_syncLock)
        {
            var produto = _repository.ObterPorCodigo(requisicao.CodigoProduto);
            if (produto is null)
                throw new ProdutoNaoEncontradoException(requisicao.CodigoProduto);

            var saldoAnterior = produto.Saldo;
            var saldoAtualizado = ExecutarAtualizacaoSaldo(produto, requisicao.Tipo, requisicao.Quantidade);

            _repository.Atualizar(produto);

            var movimentacao = new MovimentacaoEstoque(
                id: Guid.NewGuid(),
                codigoProduto: produto.Codigo,
                descricaoProduto: produto.Descricao,
                tipo: requisicao.Tipo,
                quantidade: requisicao.Quantidade,
                saldoAnterior: saldoAnterior,
                saldoAtualizado: saldoAtualizado,
                motivo: requisicao.Motivo.Trim(),
                dataHora: DateTimeOffset.UtcNow
            );

            _repository.RegistrarMovimentacao(movimentacao);

            return new ResultadoMovimentacaoDto(
                MovimentacaoId: movimentacao.Id,
                CodigoProduto: movimentacao.CodigoProduto,
                DescricaoProduto: movimentacao.DescricaoProduto,
                Tipo: movimentacao.Tipo,
                QuantidadeMovimentada: movimentacao.Quantidade,
                SaldoAnterior: movimentacao.SaldoAnterior,
                SaldoAtualizado: movimentacao.SaldoAtualizado,
                Motivo: movimentacao.Motivo,
                DataHora: movimentacao.DataHora
            );
        }
    }

    public IReadOnlyList<Produto> ObterCatalogo() => _repository.ObterTodos();

    public Produto ObterProduto(int codigoProduto)
    {
        var produto = _repository.ObterPorCodigo(codigoProduto);
        if (produto is null)
            throw new ProdutoNaoEncontradoException(codigoProduto);

        return produto;
    }

    public IReadOnlyList<MovimentacaoEstoque> ObterHistoricoMovimentacoes() => _repository.ObterHistorico();

    public void ReinicializarEstoque() => _repository.ResetarParaEstadoInicial();

    private static int ExecutarAtualizacaoSaldo(Produto produto, TipoMovimentacao tipo, int quantidade) => tipo switch
    {
        TipoMovimentacao.Entrada => produto.AdicionarSaldo(quantidade),
        TipoMovimentacao.Saida => produto.DeduzirSaldo(quantidade),
        _ => throw new MovimentacaoInvalidaException($"Tipo de movimentação '{tipo}' não é suportado.")
    };
}
