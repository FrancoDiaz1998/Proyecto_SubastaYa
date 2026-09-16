namespace SubastaYa.Application.UseCases.Billeteras.ListarMovimientos;

public sealed record MovimientoBilleteraResponse(
    long Id,
    int BilleteraId,
    int? SubastaId,
    string Tipo,
    decimal Monto,
    Guid OperacionId,
    DateTimeOffset Fecha);
