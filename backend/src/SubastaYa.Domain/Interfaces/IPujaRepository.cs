using SubastaYa.Domain.Entidades;

namespace SubastaYa.Domain.Interfaces;

public interface IPujaRepository
{
    Task<Subasta?> ObtenerSubastaParaActualizarAsync(
        int subastaId, CancellationToken cancellationToken = default);

    Task<Puja?> ObtenerPujaLiderAsync(
        int subastaId, CancellationToken cancellationToken = default);

    Task<Billetera?> ObtenerBilleteraPorUsuarioParaActualizarAsync(
        Guid usuarioId, CancellationToken cancellationToken = default);

    Task AgregarPujaAsync(Puja puja, CancellationToken cancellationToken = default);

    Task AgregarMovimientoAsync(
        MovimientoBilletera movimiento, CancellationToken cancellationToken = default);

    Task AgregarAuditoriaAsync(
        RegistroAuditoria registro, CancellationToken cancellationToken = default);
}
