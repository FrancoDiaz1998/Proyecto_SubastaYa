namespace SubastaYa.Application.Interfaces;

public interface IUnidadDeTrabajo
{
    Task EjecutarEnTransaccionAsync(
        Func<CancellationToken, Task> operacion,
        CancellationToken cancellationToken = default);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
