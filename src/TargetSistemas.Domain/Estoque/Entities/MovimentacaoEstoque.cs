using TargetSistemas.Domain.Estoque.Enums;

namespace TargetSistemas.Domain.Estoque.Entities;

/// <summary>
/// Registro auditável e imutável de movimentação de estoque com identificador único GUID.
/// </summary>
public sealed record MovimentacaoEstoque
{
    public Guid Id { get; }
    public int CodigoProduto { get; }
    public string DescricaoProduto { get; }
    public TipoMovimentacao Tipo { get; }
    public int Quantidade { get; }
    public int SaldoAnterior { get; }
    public int SaldoAtualizado { get; }
    public string Motivo { get; }
    public DateTimeOffset DataHora { get; }

    public MovimentacaoEstoque(
        Guid id,
        int codigoProduto,
        string descricaoProduto,
        TipoMovimentacao tipo,
        int quantidade,
        int saldoAnterior,
        int saldoAtualizado,
        string motivo,
        DateTimeOffset dataHora)
    {
        Id = id;
        CodigoProduto = codigoProduto;
        DescricaoProduto = descricaoProduto;
        Tipo = tipo;
        Quantidade = quantidade;
        SaldoAnterior = saldoAnterior;
        SaldoAtualizado = saldoAtualizado;
        Motivo = motivo;
        DataHora = dataHora;
    }
}
