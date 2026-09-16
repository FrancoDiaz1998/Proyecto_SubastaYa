namespace SubastaYa.Application.Common.Exceptions;

public sealed class PujaRechazadaException : Exception
{
    public PujaRechazadaException(string mensaje) : base(mensaje) { }
}
