namespace SubastaYa.Domain.Entidades;

public class Categoria
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public string? UrlIcono { get; set; }
}
