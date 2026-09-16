namespace SubastaYa.Application.UseCases.Subastas.ObtenerSubasta;

public sealed record ObtenerSubastaResponse(
    int Id,
    Guid VendedorId,
    string? VendedorNombre,
    int CategoriaId,
    string CategoriaNombre,
    string Titulo,
    string Descripcion,
    string UrlImagen,
    decimal PrecioBase,
    decimal IncrementoMinimo,
    decimal PujaActual,
    int CantidadPujas,
    DateTimeOffset FechaInicio,
    DateTimeOffset FechaFin,
    string Estado,
    Guid? PostorLiderId,
    string? PostorLider,
    IReadOnlyList<PujaResponse> HistorialPujas);

public sealed record PujaResponse(
    long Id,
    string PostorNombre,
    decimal Monto,
    DateTimeOffset Fecha);
