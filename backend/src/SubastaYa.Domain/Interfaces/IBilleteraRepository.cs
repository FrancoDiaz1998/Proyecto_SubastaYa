using SubastaYa.Domain.Entidades;

namespace SubastaYa.Domain.Interfaces;

public interface IBilleteraRepository
{
    Task<Billetera?> ObtenerPorUsuarioAsync(
        Guid usuarioId, CancellationToken cancellationToken = default);

    Task<Billetera?> ObtenerPorUsuarioParaActualizarAsync(
        Guid usuarioId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MovimientoBilletera>> ObtenerMovimientosPorUsuarioAsync(
        Guid usuarioId, int limite = 100,
        CancellationToken cancellationToken = default);

    Task AgregarMovimientoAsync(
        MovimientoBilletera movimiento,
        CancellationToken cancellationToken = default);

    Task AgregarAuditoriaAsync(
        RegistroAuditoria registro,
        CancellationToken cancellationToken = default);
}
