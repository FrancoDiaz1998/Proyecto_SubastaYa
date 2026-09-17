using System.Text.Json;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Subastas.FinalizarVencidas;

public sealed class FinalizarSubastasVencidasUseCase
{
    private readonly IFinalizacionSubastasRepository _repository;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public FinalizarSubastasVencidasUseCase(
        IFinalizacionSubastasRepository repository,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repository = repository;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<FinalizarSubastasVencidasResultado> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var ahora = DateTimeOffset.UtcNow;

        var errores = new List<string>();

        var idsProgramadas = await _repository.ObtenerIdsProgramadasParaActivarAsync(
            ahora, 50, cancellationToken);

        foreach (var subastaId in idsProgramadas)
        {
            try
            {
                await ActivarSubastaProgramadaAsync(subastaId, cancellationToken);
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                errores.Add(
                    $"Activación subasta #{subastaId}: {exception.Message}");
            }
        }

        var ids = await _repository.ObtenerIdsVencidasAsync(
            DateTimeOffset.UtcNow, 50, cancellationToken);

        var procesadas = 0;

        foreach (var subastaId in ids)
        {
            try
            {
                await ProcesarSubastaAsync(subastaId, cancellationToken);
                procesadas++;
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                errores.Add(
                    $"Finalización subasta #{subastaId}: {exception.Message}");
            }
        }

        return new FinalizarSubastasVencidasResultado(
            procesadas,
            errores.Count,
            errores);
    }

    private async Task ActivarSubastaProgramadaAsync(
        int subastaId,
        CancellationToken cancellationToken)
    {
        await _unidadDeTrabajo.EjecutarEnTransaccionAsync(async ct =>
        {
            var subasta = await _repository.ObtenerSubastaParaActualizarAsync(
                subastaId, ct);

            if (subasta is null)
                return;

            var ahora = DateTimeOffset.UtcNow;

            if (subasta.Estado != EstadoSubasta.Programada ||
                subasta.FechaInicio > ahora ||
                subasta.FechaFin <= ahora)
            {
                return;
            }

            subasta.Estado = EstadoSubasta.Activa;
            subasta.Version++;

            await _repository.AgregarAuditoriaAsync(new RegistroAuditoria
            {
                UsuarioActorId = null,
                TipoEntidad = "Subasta",
                EntidadId = subasta.Id.ToString(),
                Accion = "CambioEstado",
                DetallesJson = JsonSerializer.Serialize(new
                {
                    EstadoAnterior = EstadoSubasta.Programada.ToString(),
                    EstadoNuevo = EstadoSubasta.Activa.ToString(),
                    Motivo = "InicioProgramado"
                }),
                Fecha = ahora
            }, ct);
        }, cancellationToken);
    }

    private async Task ProcesarSubastaAsync(
        int subastaId,
        CancellationToken cancellationToken)
    {
        await _unidadDeTrabajo.EjecutarEnTransaccionAsync(async ct =>
        {
            var subasta = await _repository.ObtenerSubastaParaActualizarAsync(
                subastaId, ct);

            if (subasta is null) return;

            var ahora = DateTimeOffset.UtcNow;
            if (subasta.FechaFin > ahora ||
                (subasta.Estado != EstadoSubasta.Activa &&
                 subasta.Estado != EstadoSubasta.Programada))
                return;

            var estadoAnterior = subasta.Estado;
            var pujaLider = await _repository.ObtenerPujaLiderAsync(subasta.Id, ct);

            if (pujaLider is null)
            {
                subasta.Estado = EstadoSubasta.Desierta;
                subasta.Version++;

                await _repository.AgregarAuditoriaAsync(new RegistroAuditoria
                {
                    UsuarioActorId = null,
                    TipoEntidad = "Subasta",
                    EntidadId = subasta.Id.ToString(),
                    Accion = "CambioEstado",
                    DetallesJson = JsonSerializer.Serialize(new
                    {
                        EstadoAnterior = estadoAnterior.ToString(),
                        EstadoNuevo = EstadoSubasta.Desierta.ToString(),
                        Motivo = "VencimientoSinOfertas"
                    }),
                    Fecha = ahora
                }, ct);

                return;
            }

            var ventaExistente = await _repository.ObtenerVentaPorSubastaAsync(
                subasta.Id, ct);

            if (ventaExistente is not null)
            {
                subasta.Estado = EstadoSubasta.Finalizada;
                subasta.Version++;
                return;
            }

            var billeteraComprador = await _repository
                .ObtenerBilleteraPorUsuarioParaActualizarAsync(pujaLider.PostorId, ct)
                ?? throw new InvalidOperationException(
                    "No se encontró la billetera del comprador ganador.");

            var billeteraVendedor = await _repository
                .ObtenerBilleteraPorUsuarioParaActualizarAsync(subasta.VendedorId, ct)
                ?? throw new InvalidOperationException(
                    "No se encontró la billetera del vendedor.");

            var precioFinal = pujaLider.Monto;
            var operacionId = Guid.NewGuid();

            // Los datos semilla incluyen una subasta vencida con ganador creada antes
            // del flujo real de Escrow. Si falta parte de la retención, se regulariza
            // dentro de la misma transacción antes de liquidar la venta.
            if (billeteraComprador.SaldoRetenido < precioFinal)
            {
                var faltanteRetencion = precioFinal - billeteraComprador.SaldoRetenido;

                if (billeteraComprador.SaldoDisponible < faltanteRetencion)
                    throw new InvalidOperationException(
                        "La billetera ganadora no posee fondos suficientes para completar la liquidación.");

                billeteraComprador.SaldoRetenido += faltanteRetencion;
                billeteraComprador.Version++;

                await _repository.AgregarMovimientoAsync(new MovimientoBilletera
                {
                    BilleteraId = billeteraComprador.Id,
                    SubastaId = subasta.Id,
                    Tipo = TipoMovimientoBilletera.Retencion,
                    Monto = faltanteRetencion,
                    OperacionId = operacionId,
                    Fecha = ahora
                }, ct);

                await _repository.AgregarAuditoriaAsync(new RegistroAuditoria
                {
                    UsuarioActorId = null,
                    TipoEntidad = "Subasta",
                    EntidadId = subasta.Id.ToString(),
                    Accion = "RegularizacionEscrow",
                    DetallesJson = JsonSerializer.Serialize(new
                    {
                        CompradorId = pujaLider.PostorId,
                        MontoRegularizado = faltanteRetencion
                    }),
                    Fecha = ahora
                }, ct);
            }

            billeteraComprador.SaldoRetenido -= precioFinal;
            billeteraComprador.SaldoTotal -= precioFinal;
            billeteraComprador.Version++;

            billeteraVendedor.SaldoTotal += precioFinal;
            billeteraVendedor.Version++;

            await _repository.AgregarMovimientoAsync(new MovimientoBilletera
            {
                BilleteraId = billeteraComprador.Id,
                SubastaId = subasta.Id,
                Tipo = TipoMovimientoBilletera.Pago,
                Monto = precioFinal,
                OperacionId = operacionId,
                Fecha = ahora
            }, ct);

            await _repository.AgregarMovimientoAsync(new MovimientoBilletera
            {
                BilleteraId = billeteraVendedor.Id,
                SubastaId = subasta.Id,
                Tipo = TipoMovimientoBilletera.Cobro,
                Monto = precioFinal,
                OperacionId = operacionId,
                Fecha = ahora
            }, ct);

            await _repository.AgregarVentaAsync(new Venta
            {
                SubastaId = subasta.Id,
                CompradorId = pujaLider.PostorId,
                VendedorId = subasta.VendedorId,
                PrecioFinal = precioFinal,
                Fecha = ahora
            }, ct);

            subasta.Estado = EstadoSubasta.Finalizada;
            subasta.Version++;

            await _repository.AgregarAuditoriaAsync(new RegistroAuditoria
            {
                UsuarioActorId = null,
                TipoEntidad = "Subasta",
                EntidadId = subasta.Id.ToString(),
                Accion = "CambioEstado",
                DetallesJson = JsonSerializer.Serialize(new
                {
                    EstadoAnterior = estadoAnterior.ToString(),
                    EstadoNuevo = EstadoSubasta.Finalizada.ToString(),
                    GanadorId = pujaLider.PostorId,
                    PrecioFinal = precioFinal,
                    OperacionId = operacionId
                }),
                Fecha = ahora
            }, ct);
        }, cancellationToken);
    }
}
