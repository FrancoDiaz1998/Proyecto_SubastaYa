namespace SubastaYa.Application.UseCases.Subastas.RegistrarPuja;

public sealed record RegistrarPujaResponse(
    long Id,
    int SubastaId,
    decimal Monto,
    DateTimeOffset Fecha,
    DateTimeOffset FechaFin,
    bool ExtendidaPorAntiSniping);
