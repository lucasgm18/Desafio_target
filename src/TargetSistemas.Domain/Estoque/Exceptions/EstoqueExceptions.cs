namespace TargetSistemas.Domain.Estoque.Exceptions;

public class ProdutoNaoEncontradoException : Exception
{
    public int CodigoProduto { get; }

    public ProdutoNaoEncontradoException(int codigoProduto)
        : base($"Produto com código {codigoProduto} não foi localizado no inventário.")
    {
        CodigoProduto = codigoProduto;
    }
}

public class SaldoInsuficienteException : Exception
{
    public int CodigoProduto { get; }
    public int SaldoAtual { get; }
    public int QuantidadeSolicitada { get; }

    public SaldoInsuficienteException(int codigoProduto, int saldoAtual, int quantidadeSolicitada)
        : base($"Saldo insuficiente para saída do produto {codigoProduto}. Saldo atual: {saldoAtual}, quantidade solicitada: {quantidadeSolicitada}.")
    {
        CodigoProduto = codigoProduto;
        SaldoAtual = saldoAtual;
        QuantidadeSolicitada = quantidadeSolicitada;
    }
}

public class MovimentacaoInvalidaException : Exception
{
    public MovimentacaoInvalidaException(string mensagem) : base(mensagem) { }
}
