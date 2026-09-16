using Microsoft.EntityFrameworkCore;
using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Infrastructure.Persistencia.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly SubastaYaDbContext _dbContext;

    public UsuarioRepository(SubastaYaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Usuario?> ObtenerPorEmailAsync(
        string email, CancellationToken cancellationToken = default)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();

        return _dbContext.Usuarios.AsNoTracking()
            .SingleOrDefaultAsync(
                usuario => usuario.Email.ToLower() == emailNormalizado,
                cancellationToken);
    }
}