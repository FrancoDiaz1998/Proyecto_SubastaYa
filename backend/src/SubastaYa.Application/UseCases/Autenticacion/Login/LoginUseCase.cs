using SubastaYa.Application.Common.Exceptions;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Interfaces;

namespace SubastaYa.Application.UseCases.Autenticacion.Login;

public sealed class LoginUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginUseCase(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> EjecutarAsync(
        LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email, cancellationToken);

        if (usuario is null ||
            !_passwordHasher.VerifyPassword(usuario, usuario.PasswordHash, request.Password))
            throw new CredencialesInvalidasException();

        var (token, expiraEn) = _jwtService.GenerarToken(usuario);

        return new LoginResponse(
            token, expiraEn,
            new UsuarioSesionResponse(usuario.Id, usuario.Nombre, usuario.Email));
    }
}