namespace SubastaYa.Application.UseCases.Categorias.ListarCategorias;

public sealed record ListarCategoriasResponse(
    int Id,
    string Nombre,
    string? UrlIcono);