using System.Text.Json;
using SubastaYa.Application.Common.Exceptions;
using SubastaYa.Application.Interfaces;
using SubastaYa.Application.UseCases.Billeteras.ObtenerSaldo;
using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Billeteras.AcreditarSaldo;

public sealed class AcreditarSaldoUseCase
{
    private const decimal MaximoNumeric18DosDecimales = 9_999_999_999_999_999.99m;

    private readonly IBilleteraRepository _billeteraRepository;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public AcreditarSaldoUseCase(
        IBilleteraRepository billeteraRepository,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _billeteraRepository = billeteraRepository;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<AcreditarSaldoResponse> EjecutarAsync(
        Guid usuarioId, AcreditarSaldoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Monto <= 0)
            throw new ArgumentException(
                "El monto a acreditar debe ser mayor a cero.", nameof(request.Monto));

        if (decimal.Round(request.Monto, 2) != request.Monto)
            throw new ArgumentException(
                "El monto puede tener como máximo dos decimales.", nameof(request.Monto));

        AcreditarSaldoResponse? respuesta = null;

        await _unidadDeTrabajo.EjecutarEnTransaccionAsync(async ct =>
        {
            var billetera = await _billeteraRepository
                .ObtenerPorUsuarioParaActualizarAsync(usuarioId, ct)
                ?? throw new CuentaNoDisponibleException(
                    "El usuario no posee una billetera disponible.");

            if (billetera.SaldoTotal > MaximoNumeric18DosDecimales - request.Monto)
                throw new ArgumentException(
                    "El monto supera el saldo máximo admitido por la billetera.",
                    nameof(request.Monto));

            var ahora = DateTimeOffset.UtcNow;
            var operacionId = Guid.NewGuid();

            billetera.SaldoTotal += request.Monto;
            billetera.Version++;

            await _billeteraRepository.AgregarMovimientoAsync(new MovimientoBilletera
            {
                BilleteraId = billetera.Id,
                Tipo = TipoMovimientoBilletera.Deposito,
                Monto = request.Monto,
                OperacionId = operacionId,
                Fecha = ahora
            }, ct);

            await _billeteraRepository.AgregarAuditoriaAsync(new RegistroAuditoria
            {
                UsuarioActorId = usuarioId,
                TipoEntidad = "Billetera",
                EntidadId = billetera.Id.ToString(),
                Accion = "AcreditacionManualSaldo",
                DetallesJson = JsonSerializer.Serialize(new
                {
                    Monto = request.Monto,
                    SaldoTotalResultante = billetera.SaldoTotal,
                    OperacionId = operacionId
                }),
                Fecha = ahora
            }, ct);

            respuesta = new AcreditarSaldoResponse(
                operacionId,
                request.Monto,
                ahora,
                new ObtenerSaldoBilleteraResponse(
                    billetera.Id,
                    billetera.SaldoTotal,
                    billetera.SaldoRetenido,
                    billetera.SaldoDisponible,
                    billetera.Version));
        }, cancellationToken);

        return respuesta!;
    }
}
