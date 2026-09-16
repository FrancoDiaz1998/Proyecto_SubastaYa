using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Subastas.ObtenerSubasta;

public sealed class ObtenerSubastaUseCase
{
    private readonly ISubastaRepository _subastaRepository;

    public ObtenerSubastaUseCase(ISubastaRepository subastaRepository)
    {
        _subastaRepository = subastaRepository;
    }

    public async Task<ObtenerSubastaResponse?> EjecutarAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var subasta = await _subastaRepository.ObtenerPorIdConDetalleAsync(id, cancellationToken);
        if (subasta is null) return null;

        var pujaLider = subasta.Pujas
            .OrderByDescending(puja => puja.Monto)
            .ThenBy(puja => puja.Fecha)
            .FirstOrDefault();

        var historial = subasta.Pujas
            .OrderByDescending(puja => puja.Fecha)
            .Select(puja => new PujaResponse(
                puja.Id, ObtenerNombre(puja.Postor), puja.Monto, puja.Fecha))
            .ToList();

        return new ObtenerSubastaResponse(
            subasta.Id, subasta.VendedorId, ObtenerNombre(subasta.Vendedor),
            subasta.CategoriaId, subasta.Categoria?.Nombre ?? string.Empty,
            subasta.Titulo, subasta.Descripcion, subasta.UrlImagen,
            subasta.PrecioBase, subasta.IncrementoMinimo,
            pujaLider?.Monto ?? subasta.PrecioBase, subasta.Pujas.Count,
            subasta.FechaInicio, subasta.FechaFin, subasta.Estado.ToString(),
            pujaLider is null ? null : ObtenerNombre(pujaLider.Postor), historial);
    }

    private static string? ObtenerNombre(Usuario? usuario) =>
        usuario is null ? null :
        string.IsNullOrWhiteSpace(usuario.Nombre) ? usuario.Email : usuario.Nombre;
}