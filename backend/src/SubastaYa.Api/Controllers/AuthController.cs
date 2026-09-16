using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.UseCases.Autenticacion.Login;

namespace SubastaYa.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly LoginUseCase _login;

    public AuthController(LoginUseCase login)
    {
        _login = login;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request, CancellationToken cancellationToken)
    {
        var respuesta = await _login.EjecutarAsync(request, cancellationToken);
        return Ok(respuesta);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UsuarioSesionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UsuarioSesionResponse> Me()
    {
        var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(subject, out var usuarioId))
            return Unauthorized();

        var nombre = User.FindFirstValue(JwtRegisteredClaimNames.Name) ?? string.Empty;
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email) ?? string.Empty;

        return Ok(new UsuarioSesionResponse(usuarioId, nombre, email));
    }
}