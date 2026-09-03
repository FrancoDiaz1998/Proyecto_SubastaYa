namespace SubastaYa.Domain.Entidades;

public class Billetera
{
    public int Id { get; set; }
    public Guid UsuarioId { get; set; }
    public decimal SaldoTotal { get; set; }
    public decimal SaldoRetenido { get; set; }
    public long Version { get; set; }
}
