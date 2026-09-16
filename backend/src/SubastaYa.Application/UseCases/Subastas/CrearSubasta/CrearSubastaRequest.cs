using System.ComponentModel.DataAnnotations;

namespace SubastaYa.Application.UseCases.Subastas.CrearSubasta;

public sealed class CrearSubastaRequest
{
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Titulo { get; init; } = string.Empty;

    [Required]
    [StringLength(4000, MinimumLength = 10)]
    public string Descripcion { get; init; } = string.Empty;

    [Required]
    [Url]
    [StringLength(2048)]
    public string UrlImagen { get; init; } = string.Empty;

    public int CategoriaId { get; init; }

    public decimal PrecioBase { get; init; }

    public decimal IncrementoMinimo { get; init; }

    public DateTimeOffset FechaInicio { get; init; }

    public DateTimeOffset FechaFin { get; init; }
}
