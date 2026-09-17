using SubastaYa.Application.UseCases.Subastas.FinalizarVencidas;

namespace SubastaYa.API.BackgroundServices;

public sealed class SubastasFinalizacionWorker : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(5);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SubastasFinalizacionWorker> _logger;

    public SubastasFinalizacionWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<SubastasFinalizacionWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Intervalo);

        await EjecutarIteracionAsync(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
            await EjecutarIteracionAsync(stoppingToken);
    }

    private async Task EjecutarIteracionAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var useCase = scope.ServiceProvider
                .GetRequiredService<FinalizarSubastasVencidasUseCase>();

            var resultado = await useCase.EjecutarAsync(cancellationToken);

            if (resultado.Fallidas > 0)
            {
                foreach (var error in resultado.Errores)
                    _logger.LogWarning("No se pudo procesar una subasta: {Error}", error);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Finalización normal del host.
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error ejecutando el worker de procesamiento de subastas");
        }
    }
}
