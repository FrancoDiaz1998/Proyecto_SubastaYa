using Microsoft.EntityFrameworkCore;
using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Infrastructure.Persistencia.Repositories;

public sealed class ActividadRepository : IActividadRepository
{
    private readonly SubastaYaDbContext _dbContext;

    public ActividadRepository(SubastaYaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Subasta>> ObtenerSubastasParticipadasAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Subasta>()
            .AsNoTracking()
            .Where(subasta => subasta.Pujas.Any(puja => puja.PostorId == usuarioId))
            .Include(subasta => subasta.Categoria)
            .Include(subasta => subasta.Pujas)
            .OrderByDescending(subasta => subasta.FechaFin)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Subasta>> ObtenerSubastasPublicadasAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Subasta>()
            .AsNoTracking()
            .Where(subasta => subasta.VendedorId == usuarioId)
            .Include(subasta => subasta.Categoria)
            .Include(subasta => subasta.Pujas)
            .OrderByDescending(subasta => subasta.FechaInicio)
            .ToListAsync(cancellationToken);
    }
}
