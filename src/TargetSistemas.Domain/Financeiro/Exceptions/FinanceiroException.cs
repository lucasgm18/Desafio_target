namespace TargetSistemas.Domain.Financeiro.Exceptions;

public class FinanceiroException : Exception
{
    public FinanceiroException(string mensagem) : base(mensagem) { }
}
