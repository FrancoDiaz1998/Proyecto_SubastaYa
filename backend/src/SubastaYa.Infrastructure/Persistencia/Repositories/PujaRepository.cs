using Microsoft.EntityFrameworkCore;
using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Infrastructure.Persistencia.Repositories;

public sealed class PujaRepository : IPujaRepository
{
    private readonly SubastaYaDbContext _dbContext;

    public PujaRepository(SubastaYaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Subasta?> ObtenerSubastaParaActualizarAsync(
        int subastaId, CancellationToken cancellationToken = default) =>
        _dbContext.Set<Subasta>()
            .SingleOrDefaultAsync(subasta => subasta.Id == subastaId, cancellationToken);

    public Task<Puja?> ObtenerPujaLiderAsync(
        int subastaId, CancellationToken cancellationToken = default) =>
        _dbContext.Set<Puja>()
            .AsNoTracking()
            .Where(puja => puja.SubastaId == subastaId)
            .OrderByDescending(puja => puja.Monto)
            .ThenBy(puja => puja.Fecha)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Billetera?> ObtenerBilleteraPorUsuarioParaActualizarAsync(
        Guid usuarioId, CancellationToken cancellationToken = default) =>
        _dbContext.Set<Billetera>()
            .SingleOrDefaultAsync(billetera => billetera.UsuarioId == usuarioId, cancellationToken);

    public async Task AgregarPujaAsync(
        Puja puja, CancellationToken cancellationToken = default) =>
        await _dbContext.Set<Puja>().AddAsync(puja, cancellationToken);

    public async Task AgregarMovimientoAsync(
        MovimientoBilletera movimiento, CancellationToken cancellationToken = default) =>
        await _dbContext.Set<MovimientoBilletera>().AddAsync(movimiento, cancellationToken);

    public async Task AgregarAuditoriaAsync(
        RegistroAuditoria registro, CancellationToken cancellationToken = default) =>
        await _dbContext.Set<RegistroAuditoria>().AddAsync(registro, cancellationToken);
}
