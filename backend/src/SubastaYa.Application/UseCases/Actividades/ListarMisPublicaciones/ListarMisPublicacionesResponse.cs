namespace SubastaYa.Application.UseCases.Actividades.ListarMisPublicaciones;

public sealed record ListarMisPublicacionesResponse(
    IReadOnlyList<MisPublicacionItemResponse> Items,
    int TotalPublicaciones,
    int Activas,
    int Finalizadas,
    decimal RecaudacionTotal);

public sealed record MisPublicacionItemResponse(
    int SubastaId,
    string Titulo,
    string UrlImagen,
    string CategoriaNombre,
    string EstadoSubasta,
    string EstadoAdjudicacion,
    decimal PrecioActual,
    decimal Recaudacion,
    int CantidadPujas,
    DateTimeOffset FechaInicio,
    DateTimeOffset FechaFin);
