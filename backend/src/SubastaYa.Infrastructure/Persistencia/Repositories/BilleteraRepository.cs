using Microsoft.EntityFrameworkCore;
using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Infrastructure.Persistencia.Repositories;

public sealed class BilleteraRepository : IBilleteraRepository
{
    private readonly SubastaYaDbContext _dbContext;

    public BilleteraRepository(SubastaYaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Billetera?> ObtenerPorUsuarioAsync(
        Guid usuarioId, CancellationToken cancellationToken = default) =>
        _dbContext.Set<Billetera>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                billetera => billetera.UsuarioId == usuarioId,
                cancellationToken);

    public Task<Billetera?> ObtenerPorUsuarioParaActualizarAsync(
        Guid usuarioId, CancellationToken cancellationToken = default) =>
        _dbContext.Set<Billetera>()
            .SingleOrDefaultAsync(
                billetera => billetera.UsuarioId == usuarioId,
                cancellationToken);

    public async Task<IReadOnlyList<MovimientoBilletera>> ObtenerMovimientosPorUsuarioAsync(
        Guid usuarioId, int limite = 100,
        CancellationToken cancellationToken = default)
    {
        var limiteSeguro = Math.Clamp(limite, 1, 500);

        return await _dbContext.Set<MovimientoBilletera>()
            .AsNoTracking()
            .Where(movimiento => movimiento.Billetera!.UsuarioId == usuarioId)
            .OrderByDescending(movimiento => movimiento.Fecha)
            .ThenByDescending(movimiento => movimiento.Id)
            .Take(limiteSeguro)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarMovimientoAsync(
        MovimientoBilletera movimiento,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Set<MovimientoBilletera>()
            .AddAsync(movimiento, cancellationToken);

    public async Task AgregarAuditoriaAsync(
        RegistroAuditoria registro,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Set<RegistroAuditoria>()
            .AddAsync(registro, cancellationToken);
}
