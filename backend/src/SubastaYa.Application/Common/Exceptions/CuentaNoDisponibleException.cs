namespace SubastaYa.Application.Common.Exceptions;

public class CuentaNoDisponibleException : Exception
{
    public CuentaNoDisponibleException(string mensaje = "La cuenta de usuario no está disponible o ha sido deshabilitada.")
        : base(mensaje)
    {
    }
}

