using SubastaYa.Application.Interfaces;

namespace SubastaYa.Infrastructure.Persistencia.Repositorios;

public class UnitOfWork : IUnitOfWork
{
    private readonly SubastaYaDbContext _context;
    public ISubastaRepository Subastas { get; }
    public IBilleteraRepository Billeteras { get; }

    public UnitOfWork(
        SubastaYaDbContext context,
        ISubastaRepository subastaRepository,
        IBilleteraRepository billeteraRepository)
    {
        _context = context;
        Subastas = subastaRepository;
        Billeteras = billeteraRepository;
    }

    public async Task<int> GuardarCambiosAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task EjecutarEnTransaccionAsync(Func<Task> accion)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await accion();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}