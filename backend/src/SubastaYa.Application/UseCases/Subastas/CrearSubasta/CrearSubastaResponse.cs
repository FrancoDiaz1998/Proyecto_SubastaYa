using SubastaYa.Domain.Entidades;

namespace SubastaYa.Application.UseCases.Subastas.CrearSubasta;

public sealed record CrearSubastaResponse(
    int Id,
    Guid VendedorId,
    int CategoriaId,
    string Titulo,
    string Descripcion,
    string UrlImagen,
    decimal PrecioBase,
    decimal IncrementoMinimo,
    DateTimeOffset FechaInicio,
    DateTimeOffset FechaFin,
    EstadoSubasta Estado);
