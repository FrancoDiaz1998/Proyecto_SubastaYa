using SubastaYa.Domain.Entidades;

namespace SubastaYa.Application.UseCases.Subastas.ListarSubastas;

public sealed record ListarSubastasRequest(
    string? Busqueda = null,
    EstadoSubasta? Estado = null,
    int? CategoriaId = null,
    decimal? PrecioMin = null,
    decimal? PrecioMax = null,
    string? Orden = null,
    int Pagina = 1,
    int TamanoPagina = 20);