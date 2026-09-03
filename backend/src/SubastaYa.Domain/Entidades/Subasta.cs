namespace SubastaYa.Domain.Entidades;

public class Subasta
{
    public int Id { get; set; }
    public Guid VendedorId { get; set; }
    public int CategoriaId { get; set; }
    public required string Titulo { get; set; }
    public required string Descripcion { get; set; }
    public required string UrlImagen { get; set; }
    public decimal PrecioBase { get; set; }
    public decimal IncrementoMinimo { get; set; }
    public DateTimeOffset FechaInicio { get; set; }
    public DateTimeOffset FechaFin { get; set; }
    public EstadoSubasta Estado { get; set; }
    public long Version { get; set; }
}

public enum EstadoSubasta
{
    Programada,
    Activa,
    Finalizada,
    Desierta
}
