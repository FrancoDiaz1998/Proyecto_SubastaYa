namespace SubastaYa.Domain.Entidades;

public class Venta
{
    public int Id { get; set; }
    public int SubastaId { get; set; }
    public Guid CompradorId { get; set; }
    public Guid VendedorId { get; set; }
    public decimal PrecioFinal { get; set; }
    public DateTimeOffset Fecha { get; set; }
}
