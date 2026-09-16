using System.ComponentModel.DataAnnotations;

namespace SubastaYa.Application.UseCases.Autenticacion.Login;

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}