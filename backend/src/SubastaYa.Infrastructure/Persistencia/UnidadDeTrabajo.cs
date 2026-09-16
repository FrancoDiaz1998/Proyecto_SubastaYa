using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.Common.Exceptions;
using SubastaYa.Application.Interfaces;

namespace SubastaYa.Infrastructure.Persistencia;

public sealed class UnidadDeTrabajo : IUnidadDeTrabajo
{
    private readonly SubastaYaDbContext _dbContext;

    public UnidadDeTrabajo(SubastaYaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EjecutarEnTransaccionAsync(
        Func<CancellationToken, Task> operacion,
        CancellationToken cancellationToken = default)
    {
        await using var transaccion = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            await operacion(cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaccion.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            await transaccion.RollbackAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();
            throw new ConflictoConcurrenciaException(
                "Los datos cambiaron mientras se procesaba la operación. Actualizá e intentá nuevamente.",
                exception);
        }
        catch
        {
            await transaccion.RollbackAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();
            throw;
        }
    }

    public async Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _dbContext.ChangeTracker.Clear();
            throw new ConflictoConcurrenciaException(
                "Se detectó un conflicto de concurrencia al guardar los cambios.", exception);
        }
    }
}
