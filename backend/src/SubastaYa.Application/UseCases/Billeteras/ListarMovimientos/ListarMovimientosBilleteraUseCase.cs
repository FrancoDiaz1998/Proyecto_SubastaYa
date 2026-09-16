using SubastaYa.Application.Common.Exceptions;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Billeteras.ListarMovimientos;

public sealed class ListarMovimientosBilleteraUseCase
{
    private readonly IBilleteraRepository _billeteraRepository;

    public ListarMovimientosBilleteraUseCase(IBilleteraRepository billeteraRepository)
    {
        _billeteraRepository = billeteraRepository;
    }

    public async Task<IReadOnlyList<MovimientoBilleteraResponse>> EjecutarAsync(
        Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var billetera = await _billeteraRepository
            .ObtenerPorUsuarioAsync(usuarioId, cancellationToken)
            ?? throw new CuentaNoDisponibleException(
                "El usuario no posee una billetera disponible.");

        var movimientos = await _billeteraRepository
            .ObtenerMovimientosPorUsuarioAsync(usuarioId, 100, cancellationToken);

        return movimientos.Select(movimiento => new MovimientoBilleteraResponse(
            movimiento.Id,
            billetera.Id,
            movimiento.SubastaId,
            movimiento.Tipo.ToString(),
            movimiento.Monto,
            movimiento.OperacionId,
            movimiento.Fecha)).ToList();
    }
}
