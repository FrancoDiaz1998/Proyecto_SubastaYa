using System.ComponentModel.DataAnnotations;

namespace SubastaYa.Application.UseCases.Subastas.CrearSubasta;

public sealed record CrearSubastaRequest(
    [property: Required, StringLength(150, MinimumLength = 3)] string Titulo,
    [property: Required, StringLength(4000, MinimumLength = 10)] string Descripcion,
    [property: Required, Url, StringLength(2048)] string UrlImagen,
    int CategoriaId,
    decimal PrecioBase,
    decimal IncrementoMinimo,
    DateTimeOffset FechaInicio,
    DateTimeOffset FechaFin);
