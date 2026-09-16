using System.ComponentModel.DataAnnotations;

namespace SubastaYa.Application.UseCases.Autenticacion.Login;

public sealed record LoginRequest(
    [property: Required, EmailAddress] string Email, 
    [property: Required] string Password);