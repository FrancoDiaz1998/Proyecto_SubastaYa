namespace SubastaYa.Domain.Entidades;

public class Billetera
{
    public int Id { get; set; }
    public Guid UsuarioId { get; set; }
    public decimal SaldoTotal { get; set; }
    public decimal SaldoRetenido { get; set; }
    public long Version { get; set; }

    public decimal SaldoDisponible => SaldoTotal - SaldoRetenido;

    public Usuario? Usuario { get; set; }
    public ICollection<MovimientoBilletera> Movimientos { get; set; } = [];
}
