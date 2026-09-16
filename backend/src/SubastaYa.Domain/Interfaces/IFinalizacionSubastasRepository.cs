using SubastaYa.Domain.Entidades;

namespace SubastaYa.Domain.Interfaces;

public interface IFinalizacionSubastasRepository
{
    Task<IReadOnlyList<int>> ObtenerIdsVencidasAsync(
        DateTimeOffset ahora,
        int limite,
        CancellationToken cancellationToken = default);

    Task<Subasta?> ObtenerSubastaParaActualizarAsync(
        int subastaId,
        CancellationToken cancellationToken = default);

    Task<Puja?> ObtenerPujaLiderAsync(
        int subastaId,
        CancellationToken cancellationToken = default);

    Task<Billetera?> ObtenerBilleteraPorUsuarioParaActualizarAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default);

    Task<Venta?> ObtenerVentaPorSubastaAsync(
        int subastaId,
        CancellationToken cancellationToken = default);

    Task AgregarVentaAsync(
        Venta venta,
        CancellationToken cancellationToken = default);

    Task AgregarMovimientoAsync(
        MovimientoBilletera movimiento,
        CancellationToken cancellationToken = default);

    Task AgregarAuditoriaAsync(
        RegistroAuditoria registro,
        CancellationToken cancellationToken = default);
}
