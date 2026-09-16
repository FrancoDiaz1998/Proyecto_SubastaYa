using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.UseCases.Subastas.CrearSubasta;
using SubastaYa.Application.UseCases.Subastas.ListarSubastas;
using SubastaYa.Application.UseCases.Subastas.ObtenerSubasta;
using SubastaYa.Application.UseCases.Subastas.RegistrarPuja;

namespace SubastaYa.API.Controllers;

[ApiController]
[Route("api/v1/subastas")]
public sealed class SubastasController : ControllerBase
{
    private readonly ListarSubastasUseCase _listarSubastas;
    private readonly ObtenerSubastaUseCase _obtenerSubasta;
    private readonly CrearSubastaUseCase _crearSubasta;
    private readonly RegistrarPujaUseCase _registrarPuja;

    public SubastasController(
        ListarSubastasUseCase listarSubastas,
        ObtenerSubastaUseCase obtenerSubasta,
        CrearSubastaUseCase crearSubasta,
        RegistrarPujaUseCase registrarPuja)
    {
        _listarSubastas = listarSubastas;
        _obtenerSubasta = obtenerSubasta;
        _crearSubasta = crearSubasta;
        _registrarPuja = registrarPuja;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListarSubastasResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ListarSubastasResponse>> Listar(
        [FromQuery] ListarSubastasRequest request, CancellationToken cancellationToken)
    {
        var respuesta = await _listarSubastas.EjecutarAsync(request, cancellationToken);
        return Ok(respuesta);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ObtenerSubastaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ObtenerSubastaResponse>> ObtenerPorId(
        int id, CancellationToken cancellationToken)
    {
        var respuesta = await _obtenerSubasta.EjecutarAsync(id, cancellationToken);
        return respuesta is null ? NotFound() : Ok(respuesta);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(CrearSubastaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CrearSubastaResponse>> Crear(
        [FromBody] CrearSubastaRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetUsuarioId(out var vendedorId)) return Unauthorized();

        var respuesta = await _crearSubasta.EjecutarAsync(
            vendedorId, request, cancellationToken);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = respuesta.Id }, respuesta);
    }

    [Authorize]
    [HttpPost("{id:int}/pujas")]
    [ProducesResponseType(typeof(RegistrarPujaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegistrarPujaResponse>> RegistrarPuja(
        int id, [FromBody] RegistrarPujaRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUsuarioId(out var postorId)) return Unauthorized();

        var respuesta = await _registrarPuja.EjecutarAsync(
            id, postorId, request, cancellationToken);

        return CreatedAtAction(nameof(ObtenerPorId), new { id }, respuesta);
    }

    private bool TryGetUsuarioId(out Guid usuarioId)
    {
        var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(subject, out usuarioId);
    }
}
