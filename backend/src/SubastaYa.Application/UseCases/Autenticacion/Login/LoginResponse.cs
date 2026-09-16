namespace SubastaYa.Application.UseCases.Autenticacion.Login;

public sealed record LoginResponse(
    string Token,
    DateTimeOffset ExpiraEn,
    UsuarioSesionResponse Usuario);

public sealed record UsuarioSesionResponse(
    Guid Id,
    string Nombre,
    string Email);