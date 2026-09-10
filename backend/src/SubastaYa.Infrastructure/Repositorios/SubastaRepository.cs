using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entidades;
using SubastaYa.Infrastructure.Persistencia;

namespace SubastaYa.Infrastructure.Persistencia.Repositorios;

public class SubastaRepository : ISubastaRepository
{
    private readonly SubastaYaDbContext _context;

    public SubastaRepository(SubastaYaDbContext context)
    {
        _context = context;
    }

    public async Task<Subasta?> ObtenerPorIdConPujasAsync(int subastaId)
    {
        return await _context.Subastas
            .Include(s => s.Pujas)
            .FirstOrDefaultAsync(s => s.Id == subastaId);
    }

    public void Actualizar(Subasta subasta)
    {
        _context.Subastas.Update(subasta);
    }

    public void AgregarPuja(Puja puja)
    {
        _context.Pujas.Add(puja);
    }
}