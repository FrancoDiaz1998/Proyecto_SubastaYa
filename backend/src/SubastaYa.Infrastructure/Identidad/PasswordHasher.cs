using Microsoft.AspNetCore.Identity;
using SubastaYa.Domain.Entidades;
using SubastaYa.Application.Interfaces;

namespace SubastaYa.Infrastructure.Identidad;

public sealed class PasswordHasher: IPasswordHasher
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public string HashPassword(Usuario usuario, string password)
    {
        return _passwordHasher.HashPassword(usuario, password);
    }

    public bool VerifyPassword(Usuario usuario, string passwordHash, string providedPassword)
    {
        var resultado = _passwordHasher.VerifyHashedPassword(usuario, passwordHash, providedPassword);

        return resultado is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}