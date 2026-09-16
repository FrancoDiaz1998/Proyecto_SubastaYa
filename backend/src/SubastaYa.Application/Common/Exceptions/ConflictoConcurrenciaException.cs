namespace SubastaYa.Application.Common.Exceptions;

public sealed class ConflictoConcurrenciaException : Exception
{
    public ConflictoConcurrenciaException(string mensaje, Exception? innerException = null)
        : base(mensaje, innerException) { }
}
