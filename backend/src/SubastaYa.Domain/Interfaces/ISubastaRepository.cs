using SubastaYa.Domain.Entidades;

namespace SubastaYa.Domain.Interfaces;

public interface ISubastaRepository
{
    Task<(IReadOnlyList<Subasta> Items, int TotalCount)> ObtenerFiltradasAsync(
        string? busqueda, EstadoSubasta? estado, int? categoriaId,
        decimal? precioMin, decimal? precioMax, string? orden,
        int pagina, int tamanoPagina,
        CancellationToken cancellationToken = default);

    Task<Subasta?> ObtenerPorIdConDetalleAsync(
        int id, CancellationToken cancellationToken = default);

    Task CrearAsync(Subasta subasta, CancellationToken cancellationToken = default);
}
