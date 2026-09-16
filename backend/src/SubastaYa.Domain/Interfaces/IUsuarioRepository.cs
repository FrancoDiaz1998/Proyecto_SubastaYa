using SubastaYa.Domain.Entidades;

namespace SubastaYa.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorEmailAsync(
        string email, CancellationToken cancellationToken = default);
}