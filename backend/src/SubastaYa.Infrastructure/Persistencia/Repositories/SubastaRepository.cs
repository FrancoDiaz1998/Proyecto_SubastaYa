using Microsoft.EntityFrameworkCore;
using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Infrastructure.Persistencia.Repositories;

public sealed class SubastaRepository : ISubastaRepository
{
    private readonly SubastaYaDbContext _dbContext;

    public SubastaRepository(SubastaYaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<Subasta> Items, int TotalCount)> ObtenerFiltradasAsync(
        string? busqueda, EstadoSubasta? estado, int? categoriaId,
        decimal? precioMin, decimal? precioMax, string? orden,
        int pagina, int tamanoPagina, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Subastas.AsNoTracking()
            .Include(subasta => subasta.Categoria)
            .Include(subasta => subasta.Pujas)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var patron = $"%{busqueda.Trim()}%";
            query = query.Where(subasta =>
                EF.Functions.ILike(subasta.Titulo, patron) ||
                EF.Functions.ILike(subasta.Descripcion, patron) ||
                EF.Functions.ILike(subasta.Categoria!.Nombre, patron));
        }

        if (estado.HasValue)
        {
            query = estado == EstadoSubasta.Finalizada
                ? query.Where(subasta =>
                    subasta.Estado == EstadoSubasta.Finalizada ||
                    subasta.Estado == EstadoSubasta.Desierta)
                : query.Where(subasta => subasta.Estado == estado);
        }

        if (categoriaId.HasValue)
            query = query.Where(subasta => subasta.CategoriaId == categoriaId);

        if (precioMin.HasValue)
            query = query.Where(subasta =>
                (subasta.Pujas.Select(puja => (decimal?)puja.Monto).Max()
                    ?? subasta.PrecioBase) >= precioMin);

        if (precioMax.HasValue)
            query = query.Where(subasta =>
                (subasta.Pujas.Select(puja => (decimal?)puja.Monto).Max()
                    ?? subasta.PrecioBase) <= precioMax);

        query = orden?.Trim().ToLowerInvariant() switch
        {
            "puja_desc" => query.OrderByDescending(subasta =>
                subasta.Pujas.Select(puja => (decimal?)puja.Monto).Max()
                    ?? subasta.PrecioBase),
            _ => query.OrderBy(subasta => subasta.FechaFin)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Subasta?> ObtenerPorIdConDetalleAsync(
        int id, CancellationToken cancellationToken = default) =>
        _dbContext.Subastas.AsNoTracking()
            .Include(subasta => subasta.Categoria)
            .Include(subasta => subasta.Vendedor)
            .Include(subasta => subasta.Pujas)
                .ThenInclude(puja => puja.Postor)
            .SingleOrDefaultAsync(subasta => subasta.Id == id, cancellationToken);
}