namespace SubastaYa.Domain.Entidades;

public class Puja
{
    public long Id { get; set; }
    public int SubastaId { get; set; }
    public Subasta? Subasta { get; set; } // Propiedad de navegación

    public Guid PostorId { get; set; }
    public Usuario? Postor { get; set; }  // Propiedad de navegación

    public decimal Monto { get; set; }
    public DateTimeOffset Fecha { get; set; }
}