namespace SubastaYa.Domain.Entidades;

public class Usuario
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Billetera? Billetera { get; set; }

    public ICollection<Subasta> SubastasPublicadas { get; set; } = [];

    public ICollection<Puja> Pujas { get; set; } = [];
}