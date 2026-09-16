using SubastaYa.Domain.Entidades;

namespace SubastaYa.Domain.Interfaces;

public interface ICategoriaRepository
{
    Task<IReadOnlyList<Categoria>> ObtenerTodasAsync(CancellationToken cancellationToken = default);

    Task<Categoria?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
}

