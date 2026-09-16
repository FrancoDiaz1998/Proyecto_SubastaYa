using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.Common.Exceptions;
using SubastaYa.Application.UseCases.Billeteras.AcreditarSaldo;
using SubastaYa.Application.UseCases.Billeteras.ListarMovimientos;
using SubastaYa.Application.UseCases.Billeteras.ObtenerSaldo;

namespace SubastaYa.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/billeteras")]
public sealed class BilleterasController : ControllerBase
{
    private readonly ObtenerSaldoBilleteraUseCase _obtenerSaldo;
    private readonly ListarMovimientosBilleteraUseCase _listarMovimientos;
    private readonly AcreditarSaldoUseCase _acreditarSaldo;

    public BilleterasController(
        ObtenerSaldoBilleteraUseCase obtenerSaldo,
        ListarMovimientosBilleteraUseCase listarMovimientos,
        AcreditarSaldoUseCase acreditarSaldo)
    {
        _obtenerSaldo = obtenerSaldo;
        _listarMovimientos = listarMovimientos;
        _acreditarSaldo = acreditarSaldo;
    }

    [HttpGet("saldos")]
    [ProducesResponseType(typeof(ObtenerSaldoBilleteraResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ObtenerSaldoBilleteraResponse>> ObtenerSaldo(
        CancellationToken cancellationToken)
    {
        var respuesta = await _obtenerSaldo
            .EjecutarAsync(ObtenerUsuarioId(), cancellationToken);
        return Ok(respuesta);
    }

    [HttpGet("movimientos")]
    [ProducesResponseType(typeof(IReadOnlyList<MovimientoBilleteraResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<MovimientoBilleteraResponse>>> ListarMovimientos(
        CancellationToken cancellationToken)
    {
        var respuesta = await _listarMovimientos
            .EjecutarAsync(ObtenerUsuarioId(), cancellationToken);
        return Ok(respuesta);
    }

    [HttpPost("depositos")]
    [ProducesResponseType(typeof(AcreditarSaldoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AcreditarSaldoResponse>> Acreditar(
        AcreditarSaldoRequest request, CancellationToken cancellationToken)
    {
        var respuesta = await _acreditarSaldo
            .EjecutarAsync(ObtenerUsuarioId(), request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, respuesta);
    }

    private Guid ObtenerUsuarioId()
    {
        var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(subject, out var usuarioId)
            ? usuarioId
            : throw new CredencialesInvalidasException(
                "No se pudo identificar al usuario autenticado.");
    }
}
