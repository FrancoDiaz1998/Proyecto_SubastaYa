using Microsoft.EntityFrameworkCore;
using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Infrastructure.Persistencia.Repositories;

public sealed class CategoriaRepository : ICategoriaRepository
{
    private readonly SubastaYaDbContext _dbContext;

    public CategoriaRepository(SubastaYaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Categoria>> ObtenerTodasAsync(
        CancellationToken cancellationToken = default) =>
        await _dbContext.Categorias.AsNoTracking()
            .OrderBy(categoria => categoria.Nombre)
            .ToListAsync(cancellationToken);

    public Task<Categoria?> ObtenerPorIdAsync(
        int id, CancellationToken cancellationToken = default) =>
        _dbContext.Categorias.AsNoTracking()
            .SingleOrDefaultAsync(categoria => categoria.Id == id, cancellationToken);
}