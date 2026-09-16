namespace SubastaYa.Application.UseCases.Subastas.ListarSubastas;

public sealed record ListarSubastasResponse(
    IReadOnlyList<SubastaResumenResponse> Items,
    int Pagina,
    int TamanoPagina,
    int TotalItems,
    int TotalPaginas);

public sealed record SubastaResumenResponse(
    int Id,
    int CategoriaId,
    string CategoriaNombre,
    string Titulo,
    string UrlImagen,
    decimal PujaActual,
    int CantidadPujas,
    DateTimeOffset FechaInicio,
    DateTimeOffset FechaFin,
    string Estado);