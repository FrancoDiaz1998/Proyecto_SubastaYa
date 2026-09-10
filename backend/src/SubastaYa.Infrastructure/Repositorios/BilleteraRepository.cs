using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entidades;
using SubastaYa.Infrastructure.Persistencia;

namespace SubastaYa.Infrastructure.Persistencia.Repositorios;

public class BilleteraRepository : IBilleteraRepository
{
    private readonly SubastaYaDbContext _context;

    public BilleteraRepository(SubastaYaDbContext context)
    {
        _context = context;
    }

    public async Task<Billetera?> ObtenerPorUsuarioIdAsync(Guid usuarioId)
    {
        return await _context.Billeteras
            .FirstOrDefaultAsync(b => b.UsuarioId == usuarioId);
    }

    public void AgregarMovimiento(MovimientoBilletera movimiento)
    {
        _context.MovimientosBilletera.Add(movimiento);
    }
}