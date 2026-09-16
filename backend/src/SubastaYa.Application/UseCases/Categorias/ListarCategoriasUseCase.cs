using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Categorias.ListarCategorias;

public sealed class ListarCategoriasUseCase
{
    private readonly ICategoriaRepository _categoriaRepository;

    public ListarCategoriasUseCase(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IReadOnlyList<ListarCategoriasResponse>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var categorias = await _categoriaRepository.ObtenerTodasAsync(cancellationToken);

        return categorias.Select(categoria =>
            new ListarCategoriasResponse(
                categoria.Id, categoria.Nombre, categoria.UrlIcono)).ToList();
    }
}