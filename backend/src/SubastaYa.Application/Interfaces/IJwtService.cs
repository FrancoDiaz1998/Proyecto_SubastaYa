using SubastaYa.Domain.Entidades;

namespace SubastaYa.Application.Interfaces;

public interface IJwtService
{
    (string Token, DateTimeOffset ExpiraEn) GenerarToken(Usuario usuario);
}