namespace SubastaYa.Application.Common.Exceptions;

public class RecursoDuplicadoException : Exception
{
    public RecursoDuplicadoException(string mensaje)
        : base(mensaje)
    {
    }
}

