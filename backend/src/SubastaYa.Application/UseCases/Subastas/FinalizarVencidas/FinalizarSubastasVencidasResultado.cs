namespace SubastaYa.Application.UseCases.Subastas.FinalizarVencidas;

public sealed record FinalizarSubastasVencidasResultado(
    int Procesadas,
    int Fallidas,
    IReadOnlyList<string> Errores);
