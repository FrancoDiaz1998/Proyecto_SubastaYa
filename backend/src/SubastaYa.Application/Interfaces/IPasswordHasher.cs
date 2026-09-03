using SubastaYa.Domain.Entidades;

namespace SubastaYa.Application.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(Usuario usuario, string password);

    bool VerifyPassword(Usuario usuario, string passwordHash, string providedPassword);
}