namespace SubastaYa.Application.Common.Exceptions;

public class RecursoNoEncontradoException : Exception
{
    public RecursoNoEncontradoException(string mensaje)
        : base(mensaje)
    {
    }

    public RecursoNoEncontradoException(string entidad, object id)
        : base($"No se encontró la entidad '{entidad}' con identificador '{id}'.")
    {
    }
}

