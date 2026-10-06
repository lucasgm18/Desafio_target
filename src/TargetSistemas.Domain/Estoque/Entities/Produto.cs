using TargetSistemas.Domain.Estoque.Exceptions;

namespace TargetSistemas.Domain.Estoque.Entities;

/// <summary>
/// Representa um produto mantido no inventário do sistema ERP.
/// </summary>
public sealed class Produto
{
    public int Codigo { get; }
    public string Descricao { get; }
    public int Saldo { get; private set; }

    public Produto(int codigo, string descricao, int saldoInicial)
    {
        if (codigo <= 0)
            throw new MovimentacaoInvalidaException("O código do produto deve ser positivo.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new MovimentacaoInvalidaException("A descrição do produto não pode ser vazia.");

        if (saldoInicial < 0)
            throw new MovimentacaoInvalidaException("O saldo inicial não pode ser negativo.");

        Codigo = codigo;
        Descricao = descricao.Trim();
        Saldo = saldoInicial;
    }

    /// <summary>
    /// Adiciona quantidade ao saldo atual (Entrada de mercadoria).
    /// </summary>
    public int AdicionarSaldo(int quantidade)
    {
        if (quantidade <= 0)
            throw new MovimentacaoInvalidaException("Quantidade para entrada deve ser maior que zero.");

        Saldo += quantidade;
        return Saldo;
    }

    /// <summary>
    /// Subtrai quantidade do saldo atual (Saída de mercadoria), garantindo consistência contra saldos negativos.
    /// </summary>
    public int DeduzirSaldo(int quantidade)
    {
        if (quantidade <= 0)
            throw new MovimentacaoInvalidaException("Quantidade para saída deve ser maior que zero.");

        if (quantidade > Saldo)
            throw new SaldoInsuficienteException(Codigo, Saldo, quantidade);

        Saldo -= quantidade;
        return Saldo;
    }
}
