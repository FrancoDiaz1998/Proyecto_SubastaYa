using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.UseCases.Actividades.ListarMisPublicaciones;
using SubastaYa.Application.UseCases.Actividades.ListarMisPujas;

namespace SubastaYa.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/usuarios/me")]
public sealed class ActividadesController : ControllerBase
{
    private readonly ListarMisPujasUseCase _listarMisPujas;
    private readonly ListarMisPublicacionesUseCase _listarMisPublicaciones;

    public ActividadesController(
        ListarMisPujasUseCase listarMisPujas,
        ListarMisPublicacionesUseCase listarMisPublicaciones)
    {
        _listarMisPujas = listarMisPujas;
        _listarMisPublicaciones = listarMisPublicaciones;
    }

    [HttpGet("pujas")]
    [ProducesResponseType(typeof(ListarMisPujasResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListarMisPujasResponse>> ListarPujas(
        CancellationToken cancellationToken)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized();

        return Ok(await _listarMisPujas.EjecutarAsync(
            usuarioId, cancellationToken));
    }

    [HttpGet("subastas")]
    [ProducesResponseType(typeof(ListarMisPublicacionesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListarMisPublicacionesResponse>> ListarPublicaciones(
        CancellationToken cancellationToken)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized();

        return Ok(await _listarMisPublicaciones.EjecutarAsync(
            usuarioId, cancellationToken));
    }

    private bool TryGetUsuarioId(out Guid usuarioId)
    {
        var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(subject, out usuarioId);
    }
}
