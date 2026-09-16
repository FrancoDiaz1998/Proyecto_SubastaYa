using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Actividades.ListarMisPujas;

public sealed class ListarMisPujasUseCase
{
    private readonly IActividadRepository _actividadRepository;

    public ListarMisPujasUseCase(IActividadRepository actividadRepository)
    {
        _actividadRepository = actividadRepository;
    }

    public async Task<ListarMisPujasResponse> EjecutarAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        var subastas = await _actividadRepository.ObtenerSubastasParticipadasAsync(
            usuarioId, cancellationToken);

        var items = subastas.Select(subasta => Mapear(subasta, usuarioId)).ToList();

        return new ListarMisPujasResponse(
            items,
            items.Count,
            items.Count(item => item.EstadoSubasta == EstadoSubasta.Activa.ToString()),
            items.Count(item => item.Gane));
    }

    private static MisPujaItemResponse Mapear(Subasta subasta, Guid usuarioId)
    {
        var pujasOrdenadas = subasta.Pujas
            .OrderByDescending(puja => puja.Monto)
            .ThenBy(puja => puja.Fecha)
            .ToList();

        var pujaLider = pujasOrdenadas.FirstOrDefault();
        var miMayorPuja = subasta.Pujas
            .Where(puja => puja.PostorId == usuarioId)
            .Select(puja => (decimal?)puja.Monto)
            .Max() ?? 0m;

        var pujaActual = pujaLider?.Monto ?? subasta.PrecioBase;
        var soyLider = pujaLider?.PostorId == usuarioId;
        var gane = subasta.Estado == EstadoSubasta.Finalizada && soyLider;

        var estadoParticipacion = subasta.Estado switch
        {
            EstadoSubasta.Activa => soyLider ? "Liderando" : "Superado",
            EstadoSubasta.Finalizada => gane ? "Ganada" : "NoGanada",
            EstadoSubasta.Desierta => "Desierta",
            EstadoSubasta.Programada => "Programada",
            _ => subasta.Estado.ToString()
        };

        return new MisPujaItemResponse(
            subasta.Id,
            subasta.Titulo,
            subasta.UrlImagen,
            subasta.Categoria?.Nombre ?? string.Empty,
            subasta.Estado.ToString(),
            estadoParticipacion,
            miMayorPuja,
            pujaActual,
            soyLider,
            gane,
            subasta.Pujas.Count,
            subasta.FechaFin);
    }
}
