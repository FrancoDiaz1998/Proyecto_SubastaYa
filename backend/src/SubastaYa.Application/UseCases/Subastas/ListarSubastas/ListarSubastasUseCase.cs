using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Subastas.ListarSubastas;

public sealed class ListarSubastasUseCase
{
    private readonly ISubastaRepository _subastaRepository;

    public ListarSubastasUseCase(ISubastaRepository subastaRepository)
    {
        _subastaRepository = subastaRepository;
    }

    public async Task<ListarSubastasResponse> EjecutarAsync(
        ListarSubastasRequest request, CancellationToken cancellationToken = default)
    {
        var pagina = Math.Max(request.Pagina, 1);
        var tamanoPagina = Math.Clamp(request.TamanoPagina, 1, 100);

        var resultado = await _subastaRepository.ObtenerFiltradasAsync(
            request.Busqueda, request.Estado, request.CategoriaId,
            request.PrecioMin, request.PrecioMax, request.Orden,
            pagina, tamanoPagina, cancellationToken);

        var items = resultado.Items.Select(subasta =>
        {
            var pujaActual = subasta.Pujas
                .Select(puja => (decimal?)puja.Monto)
                .Max() ?? subasta.PrecioBase;

            return new SubastaResumenResponse(
                subasta.Id, subasta.CategoriaId,
                subasta.Categoria?.Nombre ?? string.Empty,
                subasta.Titulo, subasta.UrlImagen,
                pujaActual, subasta.Pujas.Count,
                subasta.FechaInicio, subasta.FechaFin,
                subasta.Estado.ToString());
        }).ToList();

        var totalPaginas = (int)Math.Ceiling(resultado.TotalCount / (double)tamanoPagina);

        return new ListarSubastasResponse(
            items, pagina, tamanoPagina,
            resultado.TotalCount, totalPaginas);
    }
}