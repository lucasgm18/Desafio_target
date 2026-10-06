namespace TargetSistemas.Domain.Comercial.Exceptions;

public class VendaInvalidaException : Exception
{
    public VendaInvalidaException(string mensagem) : base(mensagem) { }
}
