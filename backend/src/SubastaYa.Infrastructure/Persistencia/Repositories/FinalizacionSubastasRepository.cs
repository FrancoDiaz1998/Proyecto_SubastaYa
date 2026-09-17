using Microsoft.EntityFrameworkCore;
using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Infrastructure.Persistencia.Repositories;

public sealed class FinalizacionSubastasRepository : IFinalizacionSubastasRepository
{
    private readonly SubastaYaDbContext _dbContext;

    public FinalizacionSubastasRepository(SubastaYaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<int>> ObtenerIdsProgramadasParaActivarAsync(
        DateTimeOffset ahora,
        int limite,
        CancellationToken cancellationToken = default)
    {
        var limiteSeguro = Math.Clamp(limite, 1, 100);

        return await _dbContext.Set<Subasta>()
            .AsNoTracking()
            .Where(subasta =>
                subasta.Estado == EstadoSubasta.Programada &&
                subasta.FechaInicio <= ahora &&
                subasta.FechaFin > ahora)
            .OrderBy(subasta => subasta.FechaInicio)
            .Select(subasta => subasta.Id)
            .Take(limiteSeguro)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<int>> ObtenerIdsVencidasAsync(
        DateTimeOffset ahora,
        int limite,
        CancellationToken cancellationToken = default)
    {
        var limiteSeguro = Math.Clamp(limite, 1, 100);

        return await _dbContext.Set<Subasta>()
            .AsNoTracking()
            .Where(subasta =>
                (subasta.Estado == EstadoSubasta.Activa ||
                 subasta.Estado == EstadoSubasta.Programada) &&
                subasta.FechaFin <= ahora)
            .OrderBy(subasta => subasta.FechaFin)
            .Select(subasta => subasta.Id)
            .Take(limiteSeguro)
            .ToListAsync(cancellationToken);
    }

    public Task<Subasta?> ObtenerSubastaParaActualizarAsync(
        int subastaId,
        CancellationToken cancellationToken = default) =>
        _dbContext.Set<Subasta>()
            .SingleOrDefaultAsync(subasta => subasta.Id == subastaId, cancellationToken);

    public Task<Puja?> ObtenerPujaLiderAsync(
        int subastaId,
        CancellationToken cancellationToken = default) =>
        _dbContext.Set<Puja>()
            .AsNoTracking()
            .Where(puja => puja.SubastaId == subastaId)
            .OrderByDescending(puja => puja.Monto)
            .ThenBy(puja => puja.Fecha)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Billetera?> ObtenerBilleteraPorUsuarioParaActualizarAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default) =>
        _dbContext.Set<Billetera>()
            .SingleOrDefaultAsync(
                billetera => billetera.UsuarioId == usuarioId,
                cancellationToken);

    public Task<Venta?> ObtenerVentaPorSubastaAsync(
        int subastaId,
        CancellationToken cancellationToken = default) =>
        _dbContext.Set<Venta>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                venta => venta.SubastaId == subastaId,
                cancellationToken);

    public async Task AgregarVentaAsync(
        Venta venta,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Set<Venta>().AddAsync(venta, cancellationToken);

    public async Task AgregarMovimientoAsync(
        MovimientoBilletera movimiento,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Set<MovimientoBilletera>().AddAsync(movimiento, cancellationToken);

    public async Task AgregarAuditoriaAsync(
        RegistroAuditoria registro,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Set<RegistroAuditoria>().AddAsync(registro, cancellationToken);
}
