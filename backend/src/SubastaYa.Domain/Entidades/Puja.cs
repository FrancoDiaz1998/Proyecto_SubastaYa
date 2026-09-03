namespace SubastaYa.Domain.Entidades;

public class Puja
{
    public long Id { get; set; }
    public int SubastaId { get; set; }
    public Guid PostorId { get; set; }
    public decimal Monto { get; set; }
    public DateTimeOffset Fecha { get; set; }
}
