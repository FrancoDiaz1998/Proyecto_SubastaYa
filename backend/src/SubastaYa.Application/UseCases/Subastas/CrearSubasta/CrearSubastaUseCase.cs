using SubastaYa.Domain.Entidades;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Subastas.CrearSubasta;

public sealed class CrearSubastaUseCase
{
    private readonly ISubastaRepository _subastaRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public CrearSubastaUseCase(
        ISubastaRepository subastaRepository,
        ICategoriaRepository categoriaRepository)
    {
        _subastaRepository = subastaRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<CrearSubastaResponse> EjecutarAsync(
        Guid vendedorId, CrearSubastaRequest request,
        CancellationToken cancellationToken = default)
    {
        Validar(request);

        var categoria = await _categoriaRepository.ObtenerPorIdAsync(
            request.CategoriaId, cancellationToken);

        if (categoria is null)
            throw new ArgumentException("La categoría indicada no existe.", nameof(request.CategoriaId));

        var ahora = DateTimeOffset.UtcNow;
        var fechaInicio = request.FechaInicio.ToUniversalTime();
        var fechaFin = request.FechaFin.ToUniversalTime();

        if (fechaFin <= ahora)
            throw new ArgumentException("La fecha de finalización debe ser futura.", nameof(request.FechaFin));

        var subasta = new Subasta
        {
            VendedorId = vendedorId,
            CategoriaId = request.CategoriaId,
            Titulo = request.Titulo.Trim(),
            Descripcion = request.Descripcion.Trim(),
            UrlImagen = request.UrlImagen.Trim(),
            PrecioBase = request.PrecioBase,
            IncrementoMinimo = request.IncrementoMinimo,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Estado = fechaInicio > ahora ? EstadoSubasta.Programada : EstadoSubasta.Activa
        };

        await _subastaRepository.CrearAsync(subasta, cancellationToken);

        return new CrearSubastaResponse(
            subasta.Id, subasta.VendedorId, subasta.CategoriaId,
            subasta.Titulo, subasta.Descripcion, subasta.UrlImagen,
            subasta.PrecioBase, subasta.IncrementoMinimo,
            subasta.FechaInicio, subasta.FechaFin, subasta.Estado);
    }

    private static void Validar(CrearSubastaRequest request)
    {
        if (request.PrecioBase <= 0)
            throw new ArgumentException("El precio base debe ser mayor a cero.", nameof(request.PrecioBase));

        if (request.IncrementoMinimo <= 0)
            throw new ArgumentException("El incremento mínimo debe ser mayor a cero.", nameof(request.IncrementoMinimo));

        if (request.FechaInicio == default || request.FechaFin == default)
            throw new ArgumentException("Las fechas de inicio y finalización son obligatorias.");

        if (request.FechaFin <= request.FechaInicio)
            throw new ArgumentException("La fecha de finalización debe ser posterior a la fecha de inicio.", nameof(request.FechaFin));
    }
}
