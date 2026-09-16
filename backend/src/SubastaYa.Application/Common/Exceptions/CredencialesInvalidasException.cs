namespace SubastaYa.Application.Common.Exceptions;

public class CredencialesInvalidasException : Exception
{
    public CredencialesInvalidasException(string mensaje = "Las credenciales proporcionadas son inválidas.")
        : base(mensaje)
    {
    }
}

