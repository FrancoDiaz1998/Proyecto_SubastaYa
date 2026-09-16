using System.Text.Json;
using SubastaYa.Application.Common.Exceptions;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Subastas.RegistrarPuja;

public sealed class RegistrarPujaUseCase
{
    private readonly IPujaRepository _pujaRepository;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public RegistrarPujaUseCase(
        IPujaRepository pujaRepository,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _pujaRepository = pujaRepository;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<RegistrarPujaResponse> EjecutarAsync(
        int subastaId, Guid postorId, RegistrarPujaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Monto <= 0)
            throw new ArgumentException("El monto de la puja debe ser mayor a cero.", nameof(request.Monto));

        Puja? pujaCreada = null;
        var extendidaPorAntiSniping = false;
        var fechaFinResultante = default(DateTimeOffset);

        try
        {
            await _unidadDeTrabajo.EjecutarEnTransaccionAsync(async ct =>
            {
                var subasta = await _pujaRepository.ObtenerSubastaParaActualizarAsync(subastaId, ct)
                    ?? throw new RecursoNoEncontradoException("No se encontró la subasta indicada.");

                var ahora = DateTimeOffset.UtcNow;

                if (subasta.VendedorId == postorId)
                    throw new PujaRechazadaException("El vendedor no puede pujar en su propia subasta.");

                if (subasta.Estado == EstadoSubasta.Programada &&
                    ahora >= subasta.FechaInicio && ahora < subasta.FechaFin)
                {
                    subasta.Estado = EstadoSubasta.Activa;
                }

                if (subasta.Estado != EstadoSubasta.Activa)
                    throw new PujaRechazadaException("La subasta no está activa y no admite nuevas pujas.");

                if (ahora < subasta.FechaInicio)
                    throw new PujaRechazadaException("La subasta todavía no comenzó.");

                if (ahora >= subasta.FechaFin)
                    throw new PujaRechazadaException("La subasta ya finalizó.");

                var pujaLider = await _pujaRepository.ObtenerPujaLiderAsync(subastaId, ct);
                var pujaActual = pujaLider?.Monto ?? subasta.PrecioBase;
                var montoMinimo = pujaActual + subasta.IncrementoMinimo;

                if (request.Monto < montoMinimo)
                    throw new PujaRechazadaException(
                        $"La nueva puja debe ser de al menos {montoMinimo:0.##}.");

                var billeteraPostor = await _pujaRepository
                    .ObtenerBilleteraPorUsuarioParaActualizarAsync(postorId, ct)
                    ?? throw new CuentaNoDisponibleException("El usuario no posee una billetera disponible para pujar.");

                var mismoLider = pujaLider?.PostorId == postorId;
                var montoARetener = mismoLider
                    ? request.Monto - pujaLider!.Monto
                    : request.Monto;

                if (billeteraPostor.SaldoDisponible < montoARetener)
                    throw new CuentaNoDisponibleException(
                        $"Saldo disponible insuficiente. Se requieren {montoARetener:0.##} y tenés {billeteraPostor.SaldoDisponible:0.##} disponibles.");

                var operacionId = Guid.NewGuid();

                if (pujaLider is not null && !mismoLider)
                {
                    var billeteraLiderAnterior = await _pujaRepository
                        .ObtenerBilleteraPorUsuarioParaActualizarAsync(pujaLider.PostorId, ct)
                        ?? throw new InvalidOperationException("No se encontró la billetera del líder anterior.");

                    if (billeteraLiderAnterior.SaldoRetenido < pujaLider.Monto)
                        throw new InvalidOperationException("El saldo retenido del líder anterior es inconsistente.");

                    billeteraLiderAnterior.SaldoRetenido -= pujaLider.Monto;
                    billeteraLiderAnterior.Version++;

                    await _pujaRepository.AgregarMovimientoAsync(new MovimientoBilletera
                    {
                        BilleteraId = billeteraLiderAnterior.Id,
                        SubastaId = subasta.Id,
                        Tipo = TipoMovimientoBilletera.Liberacion,
                        Monto = pujaLider.Monto,
                        OperacionId = operacionId,
                        Fecha = ahora
                    }, ct);
                }

                billeteraPostor.SaldoRetenido += montoARetener;
                billeteraPostor.Version++;

                await _pujaRepository.AgregarMovimientoAsync(new MovimientoBilletera
                {
                    BilleteraId = billeteraPostor.Id,
                    SubastaId = subasta.Id,
                    Tipo = TipoMovimientoBilletera.Retencion,
                    Monto = montoARetener,
                    OperacionId = operacionId,
                    Fecha = ahora
                }, ct);

                pujaCreada = new Puja
                {
                    SubastaId = subasta.Id,
                    PostorId = postorId,
                    Monto = request.Monto,
                    Fecha = ahora
                };

                await _pujaRepository.AgregarPujaAsync(pujaCreada, ct);

                var fechaFinAnterior = subasta.FechaFin;
                var tiempoRestante = subasta.FechaFin - ahora;

                if (tiempoRestante > TimeSpan.Zero &&
                    tiempoRestante <= TimeSpan.FromSeconds(60))
                {
                    subasta.FechaFin = subasta.FechaFin.AddMinutes(2);
                    extendidaPorAntiSniping = true;

                    await _pujaRepository.AgregarAuditoriaAsync(new RegistroAuditoria
                    {
                        UsuarioActorId = postorId,
                        TipoEntidad = "Subasta",
                        EntidadId = subasta.Id.ToString(),
                        Accion = "ExtensionAntiSniping",
                        DetallesJson = JsonSerializer.Serialize(new
                        {
                            FechaFinAnterior = fechaFinAnterior,
                            FechaFinNueva = subasta.FechaFin,
                            MontoPuja = request.Monto
                        }),
                        Fecha = ahora
                    }, ct);
                }

                subasta.Version++;
                fechaFinResultante = subasta.FechaFin;
            }, cancellationToken);
        }
        catch (PujaRechazadaException exception)
        {
            await AuditarRechazoAsync(subastaId, postorId, request.Monto, exception.Message, cancellationToken);
            throw;
        }
        catch (CuentaNoDisponibleException exception)
        {
            await AuditarRechazoAsync(subastaId, postorId, request.Monto, exception.Message, cancellationToken);
            throw;
        }
        catch (ConflictoConcurrenciaException exception)
        {
            await AuditarRechazoAsync(subastaId, postorId, request.Monto, exception.Message, cancellationToken);
            throw;
        }

        return new RegistrarPujaResponse(
            pujaCreada!.Id,
            pujaCreada.SubastaId,
            pujaCreada.Monto,
            pujaCreada.Fecha,
            fechaFinResultante,
            extendidaPorAntiSniping);
    }

    private async Task AuditarRechazoAsync(
        int subastaId, Guid postorId, decimal monto, string motivo,
        CancellationToken cancellationToken)
    {
        await _pujaRepository.AgregarAuditoriaAsync(new RegistroAuditoria
        {
            UsuarioActorId = postorId,
            TipoEntidad = "Subasta",
            EntidadId = subastaId.ToString(),
            Accion = "PujaRechazada",
            DetallesJson = JsonSerializer.Serialize(new { Monto = monto, Motivo = motivo }),
            Fecha = DateTimeOffset.UtcNow
        }, cancellationToken);

        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
    }
}
