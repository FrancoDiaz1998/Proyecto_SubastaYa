using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.UseCases.Subastas.ListarSubastas;
using SubastaYa.Application.UseCases.Subastas.ObtenerSubasta;

namespace SubastaYa.API.Controllers;

[ApiController]
[Route("api/v1/subastas")]
public sealed class SubastasController : ControllerBase
{
    private readonly ListarSubastasUseCase _listarSubastas;
    private readonly ObtenerSubastaUseCase _obtenerSubasta;

    public SubastasController(
        ListarSubastasUseCase listarSubastas,
        ObtenerSubastaUseCase obtenerSubasta)
    {
        _listarSubastas = listarSubastas;
        _obtenerSubasta = obtenerSubasta;
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
}