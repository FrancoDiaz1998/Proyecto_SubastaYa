using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Actividades.ListarMisPublicaciones;

public sealed class ListarMisPublicacionesUseCase
{
    private readonly IActividadRepository _actividadRepository;

    public ListarMisPublicacionesUseCase(IActividadRepository actividadRepository)
    {
        _actividadRepository = actividadRepository;
    }

    public async Task<ListarMisPublicacionesResponse> EjecutarAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        var subastas = await _actividadRepository.ObtenerSubastasPublicadasAsync(
            usuarioId, cancellationToken);

        var items = subastas.Select(Mapear).ToList();

        return new ListarMisPublicacionesResponse(
            items,
            items.Count,
            items.Count(item => item.EstadoSubasta == EstadoSubasta.Activa.ToString()),
            items.Count(item => item.EstadoSubasta == EstadoSubasta.Finalizada.ToString()),
            items.Sum(item => item.Recaudacion));
    }

    private static MisPublicacionItemResponse Mapear(Subasta subasta)
    {
        var pujaActual = subasta.Pujas
            .Select(puja => (decimal?)puja.Monto)
            .Max() ?? subasta.PrecioBase;

        var recaudacion = subasta.Estado == EstadoSubasta.Finalizada
            ? pujaActual
            : 0m;

        var estadoAdjudicacion = subasta.Estado switch
        {
            EstadoSubasta.Finalizada => "Adjudicada",
            EstadoSubasta.Desierta => "SinAdjudicar",
            EstadoSubasta.Activa => "EnCurso",
            EstadoSubasta.Programada => "Programada",
            _ => subasta.Estado.ToString()
        };

        return new MisPublicacionItemResponse(
            subasta.Id,
            subasta.Titulo,
            subasta.UrlImagen,
            subasta.Categoria?.Nombre ?? string.Empty,
            subasta.Estado.ToString(),
            estadoAdjudicacion,
            pujaActual,
            recaudacion,
            subasta.Pujas.Count,
            subasta.FechaInicio,
            subasta.FechaFin);
    }
}
