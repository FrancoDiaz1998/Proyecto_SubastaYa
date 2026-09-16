using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entidades;

namespace SubastaYa.Infrastructure.Identidad;

public sealed class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTimeOffset ExpiraEn) GenerarToken(Usuario usuario)
    {
        var clave = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("No se configuró 'Jwt:Key'.");
        var issuer = _configuration["Jwt:Issuer"] ?? "SubastaYa.Api";
        var audience = _configuration["Jwt:Audience"] ?? "SubastaYa.Frontend";

        if (Encoding.UTF8.GetByteCount(clave) < 32)
            throw new InvalidOperationException("La clave JWT debe tener al menos 32 caracteres.");

        var minutos = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var valor)
            ? valor : 120;

        var ahora = DateTimeOffset.UtcNow;
        var expiraEn = ahora.AddMinutes(minutos);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, usuario.Nombre),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer, audience, claims,
            ahora.UtcDateTime, expiraEn.UtcDateTime, credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expiraEn);
    }
}