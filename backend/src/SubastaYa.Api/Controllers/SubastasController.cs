using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Servicios;

namespace SubastaYa.Api.Controllers;

[ApiController]
[Route("api/auctions")]
public class SubastasController : ControllerBase
{
    private readonly PujaService _pujaService;

    public SubastasController(PujaService pujaService)
    {
        _pujaService = pujaService;
    }

    [HttpPost("{id}/bids")]
    public async Task<IActionResult> RegistrarPuja(int id, [FromBody] RegistrarPujaDTO dto)
    {
        try
        {
            await _pujaService.RegistrarPujaAsync(id, dto);
            return Ok(new { Mensaje = "Puja registrada exitosamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Mensaje = ex.Message });
        }
    }
}