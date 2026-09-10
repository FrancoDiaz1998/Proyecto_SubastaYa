namespace SubastaYa.Domain.Entidades;

public class MovimientoBilletera
{
    public long Id { get; set; }
    public int BilleteraId { get; set; }
    public int? SubastaId { get; set; }
    public TipoMovimientoBilletera Tipo { get; set; }
    public decimal Monto { get; set; }
    public Guid OperacionId { get; set; }
    public DateTimeOffset Fecha { get; set; }
}

public enum TipoMovimientoBilletera
{
    Deposito,
    Retencion,
    Liberacion,
    Pago,
    Cobro
}
