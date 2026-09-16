using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.UseCases.Categorias.ListarCategorias;

namespace SubastaYa.API.Controllers;

[ApiController]
[Route("api/v1/categorias")]
public sealed class CategoriasController : ControllerBase
{
    private readonly ListarCategoriasUseCase _listarCategorias;

    public CategoriasController(ListarCategoriasUseCase listarCategorias)
    {
        _listarCategorias = listarCategorias;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ListarCategoriasResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ListarCategoriasResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var respuesta = await _listarCategorias.EjecutarAsync(cancellationToken);
        return Ok(respuesta);
    }
}