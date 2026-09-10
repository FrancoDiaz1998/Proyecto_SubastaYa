using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entidades;

namespace SubastaYa.Application.Servicios;

public class PujaService
{
    private readonly IUnitOfWork _unitOfWork;

    public PujaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task RegistrarPujaAsync(int subastaId, RegistrarPujaDTO dto)
    {
        await _unitOfWork.EjecutarEnTransaccionAsync(async () =>
        {
            var ahora = DateTimeOffset.UtcNow;

            // 1. Obtener Subasta
            var subasta = await _unitOfWork.Subastas.ObtenerPorIdConPujasAsync(subastaId)
                ?? throw new KeyNotFoundException("La subasta especificada no existe.");

            // 2. Validaciones básicas de subasta
            if (subasta.Estado != EstadoSubasta.Activa)
                throw new InvalidOperationException("La subasta no está activa.");

            if (subasta.FechaFin <= ahora)
                throw new InvalidOperationException("La subasta ya ha finalizado.");

            if (subasta.VendedorId == dto.PostorId)
                throw new InvalidOperationException("El vendedor no puede realizar ofertas en su propia subasta.");

            // 3. Monto mínimo requerido
            var pujaMaxima = subasta.Pujas.OrderByDescending(p => p.Monto).FirstOrDefault();
            decimal montoMinimo = pujaMaxima != null
                ? pujaMaxima.Monto + subasta.IncrementoMinimo
                : subasta.PrecioBase;

            if (dto.Monto < montoMinimo)
                throw new InvalidOperationException($"El monto ofertado debe ser de al menos {montoMinimo:C}.");

            // 4. Billetera del nuevo postor
            var billeteraNuevoPostor = await _unitOfWork.Billeteras.ObtenerPorUsuarioIdAsync(dto.PostorId)
                ?? throw new KeyNotFoundException("No se encontró la billetera del postor.");

            decimal saldoDisponible = billeteraNuevoPostor.SaldoTotal - billeteraNuevoPostor.SaldoRetenido;
            if (saldoDisponible < dto.Monto)
                throw new InvalidOperationException("Fondos insuficientes en la billetera.");

            // 5. Devolución de Escrow al postor anterior
            if (pujaMaxima != null)
            {
                var billeteraAnterior = await _unitOfWork.Billeteras.ObtenerPorUsuarioIdAsync(pujaMaxima.PostorId);
                if (billeteraAnterior != null)
                {
                    billeteraAnterior.SaldoRetenido -= pujaMaxima.Monto;
                    billeteraAnterior.Version++;

                    _unitOfWork.Billeteras.AgregarMovimiento(new MovimientoBilletera
                    {
                        BilleteraId = billeteraAnterior.Id,
                        SubastaId = subasta.Id,
                        Tipo = TipoMovimientoBilletera.Liberacion,
                        Monto = pujaMaxima.Monto,
                        OperacionId = Guid.NewGuid(),
                        Fecha = ahora
                    });
                }
            }

            // 6. Retención de Escrow al nuevo postor
            billeteraNuevoPostor.SaldoRetenido += dto.Monto;
            billeteraNuevoPostor.Version++;

            _unitOfWork.Billeteras.AgregarMovimiento(new MovimientoBilletera
            {
                BilleteraId = billeteraNuevoPostor.Id,
                SubastaId = subasta.Id,
                Tipo = TipoMovimientoBilletera.Retencion,
                Monto = dto.Monto,
                OperacionId = Guid.NewGuid(),
                Fecha = ahora
            });

            // 7. Regla Anti-Sniping (extensión de 2 minutos si quedan <= 60s)
            if ((subasta.FechaFin - ahora).TotalSeconds <= 60)
            {
                subasta.FechaFin = subasta.FechaFin.AddMinutes(2);
            }

            // 8. Registrar Puja
            _unitOfWork.Subastas.AgregarPuja(new Puja
            {
                SubastaId = subasta.Id,
                PostorId = dto.PostorId,
                Monto = dto.Monto,
                Fecha = ahora
            });

            subasta.Version++;
            _unitOfWork.Subastas.Actualizar(subasta);
        });
    }
}