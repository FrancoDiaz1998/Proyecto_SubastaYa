namespace SubastaYa.Application.UseCases.Actividades.ListarMisPujas;

public sealed record ListarMisPujasResponse(
    IReadOnlyList<MisPujaItemResponse> Items,
    int Participaciones,
    int Activas,
    int Ganadas);

public sealed record MisPujaItemResponse(
    int SubastaId,
    string Titulo,
    string UrlImagen,
    string CategoriaNombre,
    string EstadoSubasta,
    string EstadoParticipacion,
    decimal MiMayorPuja,
    decimal PujaActual,
    bool SoyLider,
    bool Gane,
    int CantidadPujas,
    DateTimeOffset FechaFin);
