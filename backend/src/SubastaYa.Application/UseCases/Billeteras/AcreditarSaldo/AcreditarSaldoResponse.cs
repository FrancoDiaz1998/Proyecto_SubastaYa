using SubastaYa.Application.UseCases.Billeteras.ObtenerSaldo;

namespace SubastaYa.Application.UseCases.Billeteras.AcreditarSaldo;

public sealed record AcreditarSaldoResponse(
    Guid OperacionId,
    decimal Monto,
    DateTimeOffset Fecha,
    ObtenerSaldoBilleteraResponse Saldo);
