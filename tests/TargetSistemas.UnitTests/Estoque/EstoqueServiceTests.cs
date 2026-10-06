using TargetSistemas.Application.Estoque.DTOs;
using TargetSistemas.Application.Estoque.Repositories;
using TargetSistemas.Application.Estoque.Services;
using TargetSistemas.Domain.Estoque.Enums;
using TargetSistemas.Domain.Estoque.Exceptions;
using Xunit;

namespace TargetSistemas.UnitTests.Estoque;

public class EstoqueServiceTests
{
    private readonly InMemoryEstoqueRepository _repository;
    private readonly EstoqueService _service;

    public EstoqueServiceTests()
    {
        _repository = new InMemoryEstoqueRepository();
        _service = new EstoqueService(_repository);
    }

    [Fact]
    public void CatalogoInicial_DeveConterOsCincoProdutosDoDesafio()
    {
        var produtos = _service.ObterCatalogo();

        Assert.Equal(5, produtos.Count);
        Assert.Contains(produtos, p => p.Codigo == 101 && p.Descricao == "Caneta Azul" && p.Saldo == 150);
        Assert.Contains(produtos, p => p.Codigo == 102 && p.Descricao == "Caderno Universitário" && p.Saldo == 75);
        Assert.Contains(produtos, p => p.Codigo == 103 && p.Descricao == "Borracha Branca" && p.Saldo == 200);
        Assert.Contains(produtos, p => p.Codigo == 104 && p.Descricao == "Lápis Preto HB" && p.Saldo == 320);
        Assert.Contains(produtos, p => p.Codigo == 105 && p.Descricao == "Marcador de Texto Amarelo" && p.Saldo == 90);
    }

    [Fact]
    public void Movimentar_EntradaValida_DeveIncrementarSaldoEGerarGuid()
    {
        var req = new RequisicaoMovimentacaoDto(
            CodigoProduto: 101,
            Tipo: TipoMovimentacao.Entrada,
            Quantidade: 50,
            Motivo: "Recebimento de Lote do Fornecedor Alfa"
        );

        var resultado = _service.Movimentar(req);

        Assert.NotEqual(Guid.Empty, resultado.MovimentacaoId);
        Assert.Equal(101, resultado.CodigoProduto);
        Assert.Equal("Caneta Azul", resultado.DescricaoProduto);
        Assert.Equal(150, resultado.SaldoAnterior);
        Assert.Equal(200, resultado.SaldoAtualizado);
        Assert.Equal(50, resultado.QuantidadeMovimentada);
        Assert.Equal(TipoMovimentacao.Entrada, resultado.Tipo);
        Assert.Equal("Recebimento de Lote do Fornecedor Alfa", resultado.Motivo);

        var produtoAtualizado = _service.ObterProduto(101);
        Assert.Equal(200, produtoAtualizado.Saldo);
    }

    [Fact]
    public void Movimentar_SaidaValida_DeveDecrementarSaldo()
    {
        var req = new RequisicaoMovimentacaoDto(
            CodigoProduto: 102,
            Tipo: TipoMovimentacao.Saida,
            Quantidade: 25,
            Motivo: "Venda Balcão Pedido #4092"
        );

        var resultado = _service.Movimentar(req);

        Assert.Equal(75, resultado.SaldoAnterior);
        Assert.Equal(50, resultado.SaldoAtualizado);

        var produtoAtualizado = _service.ObterProduto(102);
        Assert.Equal(50, produtoAtualizado.Saldo);
    }

    [Fact]
    public void Movimentar_SaidaComSaldoExato_DeveZerarEstoqueSemErro()
    {
        var req = new RequisicaoMovimentacaoDto(
            CodigoProduto: 105,
            Tipo: TipoMovimentacao.Saida,
            Quantidade: 90,
            Motivo: "Esgotamento de Lote Promocional"
        );

        var resultado = _service.Movimentar(req);

        Assert.Equal(90, resultado.SaldoAnterior);
        Assert.Equal(0, resultado.SaldoAtualizado);

        var produtoAtualizado = _service.ObterProduto(105);
        Assert.Equal(0, produtoAtualizado.Saldo);
    }

    [Fact]
    public void Movimentar_SaidaMaiorQueSaldo_DeveLancarSaldoInsuficienteExceptionEManterSaldoIntacto()
    {
        var req = new RequisicaoMovimentacaoDto(
            CodigoProduto: 102,
            Tipo: TipoMovimentacao.Saida,
            Quantidade: 80, // Saldo atual é 75
            Motivo: "Tentativa de saída excessiva"
        );

        var ex = Assert.Throws<SaldoInsuficienteException>(() => _service.Movimentar(req));
        Assert.Equal(102, ex.CodigoProduto);
        Assert.Equal(75, ex.SaldoAtual);
        Assert.Equal(80, ex.QuantidadeSolicitada);

        // Garante que o saldo não foi alterado indevidamente
        var produto = _service.ObterProduto(102);
        Assert.Equal(75, produto.Saldo);
    }

    [Fact]
    public void Movimentar_ProdutoInexistente_DeveLancarProdutoNaoEncontradoException()
    {
        var req = new RequisicaoMovimentacaoDto(
            CodigoProduto: 999,
            Tipo: TipoMovimentacao.Entrada,
            Quantidade: 10,
            Motivo: "Produto Fantasma"
        );

        Assert.Throws<ProdutoNaoEncontradoException>(() => _service.Movimentar(req));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-15)]
    public void Movimentar_QuantidadeNaoPositiva_DeveLancarMovimentacaoInvalidaException(int quantidadeInvalida)
    {
        var req = new RequisicaoMovimentacaoDto(
            CodigoProduto: 101,
            Tipo: TipoMovimentacao.Entrada,
            Quantidade: quantidadeInvalida,
            Motivo: "Ajuste de teste"
        );

        Assert.Throws<MovimentacaoInvalidaException>(() => _service.Movimentar(req));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Movimentar_MotivoVazio_DeveLancarMovimentacaoInvalidaException(string motivoInvalido)
    {
        var req = new RequisicaoMovimentacaoDto(
            CodigoProduto: 101,
            Tipo: TipoMovimentacao.Entrada,
            Quantidade: 10,
            Motivo: motivoInvalido
        );

        Assert.Throws<MovimentacaoInvalidaException>(() => _service.Movimentar(req));
    }

    [Fact]
    public void HistoricoMovimentacoes_DeveRegistrarTransacoesAuditaveis()
    {
        _service.Movimentar(new RequisicaoMovimentacaoDto(101, TipoMovimentacao.Entrada, 10, "Lote 1"));
        _service.Movimentar(new RequisicaoMovimentacaoDto(101, TipoMovimentacao.Saida, 5, "Venda 1"));

        var historico = _service.ObterHistoricoMovimentacoes();

        Assert.Equal(2, historico.Count);
        Assert.Contains(historico, h => h.Motivo == "Lote 1" && h.Tipo == TipoMovimentacao.Entrada);
        Assert.Contains(historico, h => h.Motivo == "Venda 1" && h.Tipo == TipoMovimentacao.Saida);
    }
}
