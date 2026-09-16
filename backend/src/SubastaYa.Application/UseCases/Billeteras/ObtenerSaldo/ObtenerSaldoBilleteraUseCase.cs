using SubastaYa.Application.Common.Exceptions;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Billeteras.ObtenerSaldo;

public sealed class ObtenerSaldoBilleteraUseCase
{
    private readonly IBilleteraRepository _billeteraRepository;

    public ObtenerSaldoBilleteraUseCase(IBilleteraRepository billeteraRepository)
    {
        _billeteraRepository = billeteraRepository;
    }

    public async Task<ObtenerSaldoBilleteraResponse> EjecutarAsync(
        Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var billetera = await _billeteraRepository
            .ObtenerPorUsuarioAsync(usuarioId, cancellationToken)
            ?? throw new CuentaNoDisponibleException(
                "El usuario no posee una billetera disponible.");

        return new ObtenerSaldoBilleteraResponse(
            billetera.Id,
            billetera.SaldoTotal,
            billetera.SaldoRetenido,
            billetera.SaldoDisponible,
            billetera.Version);
    }
}
