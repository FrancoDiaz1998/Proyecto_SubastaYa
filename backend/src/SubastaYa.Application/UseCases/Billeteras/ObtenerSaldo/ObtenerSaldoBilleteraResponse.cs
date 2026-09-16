namespace SubastaYa.Application.UseCases.Billeteras.ObtenerSaldo;

public sealed record ObtenerSaldoBilleteraResponse(
    int Id,
    decimal SaldoTotal,
    decimal SaldoRetenido,
    decimal SaldoDisponible,
    long Version);
