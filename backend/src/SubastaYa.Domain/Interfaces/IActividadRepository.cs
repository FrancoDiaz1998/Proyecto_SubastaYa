using SubastaYa.Domain.Entidades;

namespace SubastaYa.Domain.Interfaces;

public interface IActividadRepository
{
    Task<IReadOnlyList<Subasta>> ObtenerSubastasParticipadasAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Subasta>> ObtenerSubastasPublicadasAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default);
}
